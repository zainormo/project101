using Microsoft.AspNetCore.Mvc;

namespace WebApplication1.Controllers
{
    public class AccountController : Controller
    {
        // GET: /Account/SignIn
        [HttpGet]
        public IActionResult SignIn()
        {
            return View();
        }

        // POST: /Account/SignIn
        [HttpPost]
        public IActionResult SignIn(string email, string password, string role)
        {
            // Hardcoded admin credentials for now
            // Later this will check the database
            if (role == "Admin")
            {
                // Admin login check
                if (email == "admin@dorm.com" && password == "admin123")
                {
                    return RedirectToAction("AdminDashboard", "Home");
                }
                else
                {
                    ViewBag.Error = "Invalid admin credentials!";
                    return View();
                }
            }
            else
            {
                // Student login — redirect to student dashboard later
                if (!string.IsNullOrEmpty(email) && !string.IsNullOrEmpty(password))
                {
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    ViewBag.Error = "Invalid credentials!";
                    return View();
                }
            }
        }

        // GET: /Account/Register
        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        // POST: /Account/Register
        [HttpPost]
        public IActionResult Register(string FullName, string CNIC, string RegNumber, string Address, string SecurityPaid)
        {
            // register logic will go here later
            return RedirectToAction("Index", "Home");
        }

        // GET: /Account/RoleSelect
        [HttpGet]
        public IActionResult RoleSelect()
        {
            return View();
        }
    }
} 