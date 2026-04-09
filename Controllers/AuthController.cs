using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MeetingRoomBooking.Constants;
using MeetingRoomBooking.Data;
using MeetingRoomBooking.Models;

namespace MeetingRoomBooking.Controllers
{
    public class AuthController : Controller
    {
        private readonly AppDbContext _context;

        public AuthController(AppDbContext context)
        {
            _context = context;
        }

        public IActionResult Login()
        {
            if (HttpContext.Session.GetString(SessionKeys.UserCompany) != null)
                return RedirectToAction("Index", "Home");

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel vm)
        {
            if (!ModelState.IsValid)
                return View(vm);

            var user = await _context.AppUsers
                .FirstOrDefaultAsync(u =>
                    u.User_Logon == vm.Email &&
                    u.User_Password == vm.Password &&
                    u.User_IsActive == 1);

            if (user == null)
            {
                ModelState.AddModelError("", "Invalid email or password.");
                return View(vm);
            }

            HttpContext.Session.SetInt32(SessionKeys.UserId, user.User_Id);
            HttpContext.Session.SetString(SessionKeys.UserName, $"{user.User_FName} {user.User_LName}");
            HttpContext.Session.SetString(SessionKeys.UserEmail, user.User_Logon);
            HttpContext.Session.SetString(SessionKeys.UserCompany, user.User_Company);
            HttpContext.Session.SetString(SessionKeys.UserRole, user.User_Role);

            return RedirectToAction("Index", "Home");
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            Response.Cookies.Delete(".AspNetCore.Session");
            Response.Headers["Cache-Control"] = "no-cache, no-store, must-revalidate, private";
            Response.Headers["Pragma"] = "no-cache";
            Response.Headers["Expires"] = "0";
            return RedirectToAction("Login");
        }
    }
}