using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MeetingRoomBooking.Constants;
using MeetingRoomBooking.Data;
using MeetingRoomBooking.Filters;
using MeetingRoomBooking.Models;

namespace MeetingRoomBooking.Controllers
{
    [RequireLogin]
    [RequireAdmin]
    [NoCache]
    public class RoomController : Controller
    {
        private readonly AppDbContext _context;

        private static readonly string[] CompanyList =
        {
            "Kia Jordan", "Kia Iraq", "GAC Jordan",
            "GAC Iraq",   "BYD Iraq", "Isuzu"
        };

        public RoomController(AppDbContext context)
        {
            _context = context;
        }

        private string GetCompany() =>
            HttpContext.Session.GetString(SessionKeys.UserCompany)!;

        public async Task<IActionResult> Index()
        {
            var now = DateTime.Now;

            var rooms = await _context.MeetingRooms
                .Where(r => r.Room_IsActive == 1)
                .ToListAsync();

            var occupiedRoomNames = await _context.Communications
                .Where(c =>
                    c.Comm_Deleted == 0 &&
                    c.Comm_Status != BookingStatus.Cancelled &&
                    c.Comm_DateTime <= now &&
                    c.Comm_ToDateTime > now)
                .Select(c => c.Comm_MeetingRoom)
                .Distinct()
                .ToListAsync();

            ViewBag.OccupiedRooms = occupiedRoomNames;
            ViewBag.UserCompany = GetCompany();
            ViewBag.Companies = CompanyList;

            return View(rooms);
        }

        public IActionResult Create()
        {
            ViewBag.Companies = CompanyList;
            return View(new RoomViewModel { Room_Company = GetCompany() });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(RoomViewModel vm)
        {
            var company = vm.Room_Company ?? GetCompany();

            var exists = await _context.MeetingRooms.AnyAsync(r =>
                r.Room_Name == vm.Room_Name &&
                r.Room_Company == company &&
                r.Room_IsActive == 1);

            if (exists)
                ModelState.AddModelError("Room_Name",
                    "A room with this name already exists in that company.");

            if (!ModelState.IsValid)
            {
                vm.Room_Company = company;
                ViewBag.Companies = CompanyList;
                return View(vm);
            }

            var room = new MeetingRoom
            {
                Room_Name = vm.Room_Name.Trim(),
                Room_Location = vm.Room_Location?.Trim(),
                Room_Capacity = vm.Room_Capacity,
                Room_Amenities = vm.Room_Amenities?.Trim(),
                Room_Company = company,
                Room_IsActive = 1
            };

            _context.MeetingRooms.Add(room);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = $"✅ Room \"{room.Room_Name}\" created successfully!";
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Edit(int id)
        {
            var room = await _context.MeetingRooms.FindAsync(id);
            if (room == null) return RedirectToAction(nameof(Index));

            ViewBag.Companies = CompanyList;

            return View(new RoomViewModel
            {
                Room_Id = room.Room_Id,
                Room_Name = room.Room_Name,
                Room_Location = room.Room_Location,
                Room_Capacity = room.Room_Capacity,
                Room_Amenities = room.Room_Amenities,
                Room_Company = room.Room_Company
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, RoomViewModel vm)
        {
            var room = await _context.MeetingRooms.FindAsync(id);
            if (room == null) return RedirectToAction(nameof(Index));

            var company = vm.Room_Company ?? room.Room_Company;

            var exists = await _context.MeetingRooms.AnyAsync(r =>
                r.Room_Name == vm.Room_Name &&
                r.Room_Company == company &&
                r.Room_IsActive == 1 &&
                r.Room_Id != id);

            if (exists)
                ModelState.AddModelError("Room_Name",
                    "A room with this name already exists in that company.");

            if (!ModelState.IsValid)
            {
                vm.Room_Company = company;
                ViewBag.Companies = CompanyList;
                return View(vm);
            }

            room.Room_Name = vm.Room_Name.Trim();
            room.Room_Location = vm.Room_Location?.Trim();
            room.Room_Capacity = vm.Room_Capacity;
            room.Room_Amenities = vm.Room_Amenities?.Trim();
            room.Room_Company = company;

            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = $"✅ Room \"{room.Room_Name}\" updated successfully!";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var room = await _context.MeetingRooms.FindAsync(id);
            if (room != null)
            {
                room.Room_IsActive = 0;
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = $"Room \"{room.Room_Name}\" has been removed.";
            }
            return RedirectToAction(nameof(Index));
        }
    }
}