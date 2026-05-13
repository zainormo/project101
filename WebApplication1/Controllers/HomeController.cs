using Microsoft.AspNetCore.Mvc;

namespace DormitoryMS.Controllers;

public class HomeController : Controller
{
    public IActionResult Index()
    {
        return View();
    }

    public IActionResult Error(int? statusCode)
    {
        if (statusCode == 404)
        {
            ViewBag.ErrorCode = "404";
            ViewBag.ErrorMessage = "Page Not Found";
            ViewBag.ErrorDescription = "The page you're looking for doesn't exist or has been moved.";
        }
        else if (statusCode == 403)
        {
            ViewBag.ErrorCode = "403";
            ViewBag.ErrorMessage = "Access Denied";
            ViewBag.ErrorDescription = "You don't have permission to access this resource.";
        }
        else
        {
            ViewBag.ErrorCode = "500";
            ViewBag.ErrorMessage = "Something Went Wrong";
            ViewBag.ErrorDescription = "An unexpected error occurred. Please try again or contact the administrator.";
        }
        return View();
    }
}
