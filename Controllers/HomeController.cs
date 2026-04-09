using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MeetingRoomBooking.Constants;
using MeetingRoomBooking.Data;
using MeetingRoomBooking.Filters;

namespace MeetingRoomBooking.Controllers
{
    [RequireLogin]
    [NoCache]
    public class HomeController : Controller
    {
        private readonly AppDbContext _context;

        public HomeController(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var company = HttpContext.Session.GetString(SessionKeys.UserCompany)!;
            var now = DateTime.Now;

            var totalRooms = await _context.MeetingRooms
                .CountAsync(r => r.Room_IsActive == 1 && r.Room_Company == company);

            var occupiedRoomNames = await _context.Communications
                .Where(c =>
                    c.Comm_Deleted == 0 &&
                    c.Comm_Status != BookingStatus.Cancelled &&
                    c.Comm_DateTime <= now &&
                    c.Comm_ToDateTime > now)
                .Select(c => c.Comm_MeetingRoom)
                .Distinct()
                .ToListAsync();

            var companyRoomNames = await _context.MeetingRooms
                .Where(r => r.Room_IsActive == 1 && r.Room_Company == company)
                .Select(r => r.Room_Name)
                .ToListAsync();

            var occupiedCount = occupiedRoomNames.Count(n => companyRoomNames.Contains(n));
            var availableNow = Math.Max(0, totalRooms - occupiedCount);

            var communications = await _context.Communications
                .Where(c =>
                    c.Comm_Deleted == 0 &&
                    c.Comm_Status != BookingStatus.Cancelled &&
                    companyRoomNames.Contains(c.Comm_MeetingRoom))
                .ToListAsync();

            var users = await _context.AppUsers
                .Where(u => u.User_IsActive == 1)
                .ToListAsync();

            var calendarBookings = communications.Select(c =>
            {
                var organizer = users.FirstOrDefault(u =>
                    $"{u.User_FName} {u.User_LName}" == c.Comm_Organizer);

                var liveStatus = c.Comm_Status == BookingStatus.Cancelled
                    ? BookingStatus.Cancelled
                    : c.Comm_DateTime.HasValue && c.Comm_ToDateTime.HasValue &&
                      now >= c.Comm_DateTime.Value && now <= c.Comm_ToDateTime.Value
                        ? BookingStatus.InRoom
                        : c.Comm_ToDateTime.HasValue && now > c.Comm_ToDateTime.Value
                            ? BookingStatus.Finished
                            : BookingStatus.Confirmed;

                return new
                {
                    id = c.Comm_CommunicationId,
                    title = c.Comm_MeetingRoom ?? "Meeting",
                    start = c.Comm_DateTime,
                    end = c.Comm_ToDateTime,
                    room = c.Comm_MeetingRoom,
                    organizer = c.Comm_Organizer,
                    description = c.Comm_Description,
                    location = c.Comm_Location,
                    orgPhone = organizer?.User_Phone,
                    orgEmail = organizer?.User_Email,
                    orgDept = organizer?.User_Department,
                    status = liveStatus
                };
            }).ToList();

            var roomsList = await _context.MeetingRooms
                .Where(r => r.Room_IsActive == 1 && r.Room_Company == company)
                .Select(r => new { r.Room_Id, r.Room_Name })
                .ToListAsync();

            ViewBag.AvailableNow = availableNow;
            ViewBag.TotalRooms = totalRooms;
            ViewBag.CalendarEvents = System.Text.Json.JsonSerializer.Serialize(calendarBookings);
            ViewBag.RoomsList = roomsList;
            ViewBag.UserName = HttpContext.Session.GetString(SessionKeys.UserName);
            ViewBag.UserCompany = company;

            return View();
        }
    }
}