using MeetingRoomBooking.Data;
using MeetingRoomBooking.Filters;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

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
            var company = HttpContext.Session.GetString("UserCompany");
            if (string.IsNullOrEmpty(company))
                return RedirectToAction("Login", "Auth");

            var now = DateTime.Now;

            var totalRooms = await _context.MeetingRooms
                .CountAsync(r => r.Room_IsActive == 1 && r.Room_Company == company);

            var occupiedRoomNames = await _context.Communications
                .Where(c =>
                    c.Comm_Deleted == 0 &&
                    c.Comm_Status != "Cancelled" &&
                    c.Comm_DateTime <= now &&
                    c.Comm_ToDateTime > now)
                .Select(c => c.Comm_MeetingRoom)
                .Distinct()
                .ToListAsync();

            var companyRoomNames = await _context.MeetingRooms
                .Where(r => r.Room_IsActive == 1 && r.Room_Company == company)
                .Select(r => r.Room_Name)
                .ToListAsync();

            var occupiedCount = occupiedRoomNames
                .Count(n => companyRoomNames.Contains(n));

            var availableNow = Math.Max(0, totalRooms - occupiedCount);

            var rawBookings = await _context.Communications
                .Where(c =>
                    c.Comm_Deleted == 0 &&
                    c.Comm_Status != "Cancelled" &&
                    companyRoomNames.Contains(c.Comm_MeetingRoom))
                .Select(c => new
                {
                    id = c.Comm_CommunicationId,
                    title = c.Comm_Subject ?? "Meeting",
                    start = c.Comm_DateTime,
                    end = c.Comm_ToDateTime,
                    room = c.Comm_MeetingRoom,
                    organizer = c.Comm_Organizer,
                    description = c.Comm_Description,
                    location = c.Comm_Location,
                    dbStatus = c.Comm_Status
                })
                .ToListAsync();

            var calendarBookings = rawBookings.Select(c => new
            {
                c.id,
                c.title,
                c.start,
                c.end,
                c.room,
                c.organizer,
                c.description,
                c.location,
                status = c.dbStatus == "Cancelled"
                            ? "Cancelled"
                            : c.start.HasValue && c.end.HasValue &&
                              now >= c.start.Value && now <= c.end.Value
                                ? "In Room"
                                : c.end.HasValue && now > c.end.Value
                                    ? "Finished"
                                    : "Confirmed"
            }).ToList();

            var roomsList = await _context.MeetingRooms
                .Where(r => r.Room_IsActive == 1 && r.Room_Company == company)
                .Select(r => new { r.Room_Id, r.Room_Name })
                .ToListAsync();

            ViewBag.AvailableNow = availableNow;
            ViewBag.TotalRooms = totalRooms;
            ViewBag.CalendarEvents = System.Text.Json.JsonSerializer.Serialize(calendarBookings);
            ViewBag.RoomsList = roomsList;
            ViewBag.UserName = HttpContext.Session.GetString("UserName");
            ViewBag.UserCompany = company;

            return View();
        }
    }
}