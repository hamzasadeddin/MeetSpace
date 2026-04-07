using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace MeetingRoomBooking.Filters
{
    public class RequireLoginAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            var company = context.HttpContext.Session.GetString("UserCompany");

            if (string.IsNullOrEmpty(company))
            {
                context.Result = new RedirectToActionResult("Login", "Auth", null);
                return;
            }

            context.HttpContext.Response.Headers["Cache-Control"] =
                "no-cache, no-store, must-revalidate, private";
            context.HttpContext.Response.Headers["Pragma"] = "no-cache";
            context.HttpContext.Response.Headers["Expires"] = "0";

            base.OnActionExecuting(context);
        }
    }
}