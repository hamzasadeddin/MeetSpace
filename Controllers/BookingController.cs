using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MeetingRoomBooking.Data;
using MeetingRoomBooking.Filters;
using MeetingRoomBooking.Models;

namespace MeetingRoomBooking.Controllers
{
    [RequireLogin]
    [NoCache]
    public class BookingController : Controller
    {
        private readonly AppDbContext _context;

        public BookingController(AppDbContext context)
        {
            _context = context;
        }

        private string GetCompany() => HttpContext.Session.GetString("UserCompany")!;
        private string GetUserName() => HttpContext.Session.GetString("UserName")!;

        public async Task<IActionResult> Index()
        {
            var company = GetCompany();
            var now = DateTime.Now;

            var rooms = await _context.MeetingRooms
                .Where(r => r.Room_IsActive == 1 && r.Room_Company == company)
                .ToListAsync();

            var occupiedRoomNames = await _context.Communications
                .Where(c =>
                    c.Comm_Deleted == 0 &&
                    c.Comm_Status != "Cancelled" &&
                    c.Comm_DateTime <= now &&
                    c.Comm_ToDateTime > now)
                .Select(c => c.Comm_MeetingRoom)
                .Distinct()
                .ToListAsync();

            ViewBag.OccupiedRooms = occupiedRoomNames;
            ViewBag.UserCompany = company;

            return View(rooms);
        }

        public async Task<IActionResult> Create(int? roomId, string? startDate = null)
        {
            var company = GetCompany();
            var userName = GetUserName();

            DateTime start = DateTime.Now.AddHours(1);
            DateTime end = DateTime.Now.AddHours(2);

            if (!string.IsNullOrEmpty(startDate) &&
                DateTime.TryParse(startDate, out var parsed))
            {
                start = parsed;
                end = parsed.AddHours(1);
            }

            var vm = new BookingViewModel
            {
                StartDateTime = start,
                EndDateTime = end,
                OrganizerName = userName,
                AvailableRooms = await _context.MeetingRooms
                    .Where(r => r.Room_IsActive == 1 && r.Room_Company == company)
                    .ToListAsync()
            };

            if (roomId.HasValue)
            {
                var room = await _context.MeetingRooms.FindAsync(roomId.Value);
                if (room != null && room.Room_Company == company)
                {
                    vm.RoomId = roomId.Value;
                    vm.Room = room;
                }
            }

            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(BookingViewModel vm)
        {
            var company = GetCompany();
            var userName = GetUserName();

            vm.OrganizerName = userName;

            async Task<IActionResult> ReturnWithError(string error)
            {
                ModelState.AddModelError("", error);
                vm.AvailableRooms = await _context.MeetingRooms
                    .Where(r => r.Room_IsActive == 1 && r.Room_Company == company)
                    .ToListAsync();
                vm.Room = await _context.MeetingRooms.FindAsync(vm.RoomId);
                return View(vm);
            }

            if (!ModelState.IsValid)
            {
                vm.AvailableRooms = await _context.MeetingRooms
                    .Where(r => r.Room_IsActive == 1 && r.Room_Company == company)
                    .ToListAsync();
                vm.Room = await _context.MeetingRooms.FindAsync(vm.RoomId);
                return View(vm);
            }

            if (vm.EndDateTime <= vm.StartDateTime)
                return await ReturnWithError("⚠️ End time must be after start time.");

            if (vm.StartDateTime < DateTime.Now.AddMinutes(-5))
                return await ReturnWithError("⚠️ You cannot book a room in the past.");

            var selectedRoom = await _context.MeetingRooms.FindAsync(vm.RoomId);
            if (selectedRoom == null || selectedRoom.Room_Company != company)
                return await ReturnWithError("⚠️ Selected room was not found or does not belong to your company.");

            var duration = vm.EndDateTime - vm.StartDateTime;
            var slots = new List<(DateTime Start, DateTime End)>
            {
                (vm.StartDateTime, vm.EndDateTime)
            };

            if (vm.IsRecurring)
            {
                if (string.IsNullOrEmpty(vm.RecurrenceType))
                    return await ReturnWithError("⚠️ Please select a recurrence frequency.");

                if (!vm.RecurrenceEndDate.HasValue)
                    return await ReturnWithError("⚠️ Please select an end date for the recurring booking.");

                if (vm.RecurrenceEndDate.Value.Date <= vm.StartDateTime.Date)
                    return await ReturnWithError("⚠️ Recurrence end date must be after the booking start date.");

                var current = vm.StartDateTime;

                while (true)
                {
                    current = vm.RecurrenceType switch
                    {
                        "daily" => current.AddDays(1),
                        "weekly" => current.AddDays(7),
                        "monthly" => current.AddMonths(1),
                        _ => vm.RecurrenceEndDate.Value.AddDays(1)
                    };

                    if (current.Date > vm.RecurrenceEndDate.Value.Date) break;
                    slots.Add((current, current + duration));
                }

                if (slots.Count > 366)
                    return await ReturnWithError("⚠️ Recurrence creates too many bookings (max 366). Please shorten the date range.");
            }

            var conflictingSlots = new List<(DateTime Start, DateTime End)>();

            foreach (var (start, end) in slots)
            {
                var hasConflict = await _context.Communications.AnyAsync(c =>
                    c.Comm_MeetingRoom == selectedRoom.Room_Name &&
                    c.Comm_Deleted == 0 &&
                    c.Comm_Status != "Cancelled" &&
                    c.Comm_DateTime < end &&
                    c.Comm_ToDateTime > start
                );
                if (hasConflict)
                    conflictingSlots.Add((start, end));
            }

            if (conflictingSlots.Any())
            {
                var conflictList = string.Join(", ", conflictingSlots
                    .Take(3)
                    .Select(s => s.Start.ToString("MMM dd HH:mm")));
                var more = conflictingSlots.Count > 3
                    ? $" and {conflictingSlots.Count - 3} more..." : "";

                return await ReturnWithError(
                    $"⚠️ \"{selectedRoom.Room_Name}\" is already booked during: {conflictList}{more}. " +
                    "Please choose a different time or room.");
            }

            var bookings = slots.Select(slot => new Communication
            {
                Comm_Type = "Meeting",
                Comm_Action = "Book",
                Comm_Status = "Confirmed",
                Comm_Subject = vm.Subject,
                Comm_Organizer = userName,
                Comm_DateTime = slot.Start,
                Comm_ToDateTime = slot.End,
                Comm_Location = vm.Location ?? selectedRoom.Room_Location,
                Comm_MeetingRoom = selectedRoom.Room_Name,
                Comm_Description = vm.Description,
                Comm_CreatedDate = DateTime.Now,
                Comm_TimeStamp = DateTime.Now,
                Comm_Deleted = 0,
                Comm_IsAllDayEvent = "N",
                Comm_MeetingID = Guid.NewGuid().ToString()[..20],
                Comm_RecurrenceRule = vm.IsRecurring ? vm.RecurrenceType : null,
            }).ToList();

            _context.Communications.AddRange(bookings);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = vm.IsRecurring
                ? $"✅ Recurring meeting \"{vm.Subject}\" booked — {bookings.Count} session(s) created!"
                : $"✅ \"{vm.Subject}\" has been booked in {selectedRoom.Room_Name}!";

            return RedirectToAction(nameof(MyBookings));
        }

        public async Task<IActionResult> MyBookings()
        {
            var userName = GetUserName();

            var bookings = await _context.Communications
                .Where(c =>
                    c.Comm_Type == "Meeting" &&
                    c.Comm_Deleted == 0 &&
                    c.Comm_Organizer == userName)
                .OrderByDescending(c => c.Comm_DateTime)
                .ToListAsync();

            ViewBag.UserCompany = GetCompany();

            return View(bookings);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Cancel(int id)
        {
            var userName = GetUserName();
            var company = GetCompany();

            var booking = await _context.Communications.FindAsync(id);

            if (booking != null && booking.Comm_Organizer == userName)
            {
                booking.Comm_Status = "Cancelled";
                booking.Comm_Deleted = 1;
                booking.Comm_UpdatedDate = DateTime.Now;
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Booking cancelled successfully.";
            }

            return RedirectToAction(nameof(MyBookings));
        }
    }
}