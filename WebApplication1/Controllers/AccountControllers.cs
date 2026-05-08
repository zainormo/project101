using Microsoft.AspNetCore.Mvc;

namespace WebApplication1.Controllers
{
    public class AccountController : Controller
    {
        // GET: /Account/SignIn
        public IActionResult SignIn()
        {
            return View();
        }

        // POST: /Account/SignIn
        [HttpPost]
        public IActionResult SignIn(string email, string password)
        {
            // login logic will go here later
            return RedirectToAction("Index", "Home");
        }

        // GET: /Account/Register
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
    }
}