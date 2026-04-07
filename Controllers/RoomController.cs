using MeetingRoomBooking.Data;
using MeetingRoomBooking.Filters;
using MeetingRoomBooking.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace MeetingRoomBooking.Controllers
{
    [RequireLogin]
    [NoCache]
    public class RoomController : Controller
    {
        private readonly AppDbContext _context;

        public RoomController(AppDbContext context)
        {
            _context = context;
        }

        private string? GetCompany() =>
            HttpContext.Session.GetString("UserCompany");

        private IActionResult RedirectIfNotLoggedIn()
        {
            if (string.IsNullOrEmpty(GetCompany()))
                return RedirectToAction("Login", "Auth");
            return null!;
        }

        public async Task<IActionResult> Index()
        {
            var redirect = RedirectIfNotLoggedIn();
            if (redirect != null) return redirect;

            var company = GetCompany()!;
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

        public IActionResult Create()
        {
            var redirect = RedirectIfNotLoggedIn();
            if (redirect != null) return redirect;

            var vm = new RoomViewModel
            {
                Room_Company = GetCompany()
            };

            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(RoomViewModel vm)
        {
            var redirect = RedirectIfNotLoggedIn();
            if (redirect != null) return redirect;

            var company = GetCompany()!;

            var exists = await _context.MeetingRooms.AnyAsync(r =>
                r.Room_Name == vm.Room_Name &&
                r.Room_Company == company &&
                r.Room_IsActive == 1);

            if (exists)
            {
                ModelState.AddModelError("Room_Name",
                    "A room with this name already exists in your company.");
            }

            if (!ModelState.IsValid)
            {
                vm.Room_Company = company;
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
            var redirect = RedirectIfNotLoggedIn();
            if (redirect != null) return redirect;

            var company = GetCompany()!;
            var room = await _context.MeetingRooms.FindAsync(id);

            if (room == null || room.Room_Company != company)
                return RedirectToAction(nameof(Index));

            var vm = new RoomViewModel
            {
                Room_Id = room.Room_Id,
                Room_Name = room.Room_Name,
                Room_Location = room.Room_Location,
                Room_Capacity = room.Room_Capacity,
                Room_Amenities = room.Room_Amenities,
                Room_Company = room.Room_Company
            };

            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, RoomViewModel vm)
        {
            var redirect = RedirectIfNotLoggedIn();
            if (redirect != null) return redirect;

            var company = GetCompany()!;
            var room = await _context.MeetingRooms.FindAsync(id);

            if (room == null || room.Room_Company != company)
                return RedirectToAction(nameof(Index));

            var exists = await _context.MeetingRooms.AnyAsync(r =>
                r.Room_Name == vm.Room_Name &&
                r.Room_Company == company &&
                r.Room_IsActive == 1 &&
                r.Room_Id != id);

            if (exists)
            {
                ModelState.AddModelError("Room_Name",
                    "A room with this name already exists in your company.");
            }

            if (!ModelState.IsValid)
            {
                vm.Room_Company = company;
                return View(vm);
            }

            room.Room_Name = vm.Room_Name.Trim();
            room.Room_Location = vm.Room_Location?.Trim();
            room.Room_Capacity = vm.Room_Capacity;
            room.Room_Amenities = vm.Room_Amenities?.Trim();

            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = $"✅ Room \"{room.Room_Name}\" updated successfully!";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var redirect = RedirectIfNotLoggedIn();
            if (redirect != null) return redirect;

            var company = GetCompany()!;
            var room = await _context.MeetingRooms.FindAsync(id);

            if (room != null && room.Room_Company == company)
            {
                room.Room_IsActive = 0;
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = $"Room \"{room.Room_Name}\" has been removed.";
            }

            return RedirectToAction(nameof(Index));
        }
    }
}