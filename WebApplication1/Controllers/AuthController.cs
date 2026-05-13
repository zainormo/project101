using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using DormitoryMS.Core.Interfaces;
using DormitoryMS.ViewModels.Auth;

namespace DormitoryMS.Controllers;

public class AuthController : Controller
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    // GET: /Auth/RoleSelect
    public IActionResult RoleSelect()
    {
        return View();
    }

    // GET: /Auth/StudentOptions
    public IActionResult StudentOptions()
    {
        return View();
    }

    // GET: /Auth/AdminLogin
    public IActionResult AdminLogin()
    {
        return View();
    }

    // POST: /Auth/AdminLogin
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> AdminLogin(AdminLoginViewModel model)
    {
        if (!ModelState.IsValid)
            return View(model);

        var admin = await _authService.LoginAdminAsync(model.Username, model.Password);
        if (admin == null)
        {
            TempData["Error"] = "Invalid username or password.";
            return View(model);
        }

        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, admin.AdminId.ToString()),
            new Claim(ClaimTypes.Name, admin.FullName),
            new Claim("Username", admin.Username),
            new Claim(ClaimTypes.Role, admin.Role)
        };

        var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
        var principal = new ClaimsPrincipal(identity);

        await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);
        TempData["Success"] = $"Welcome back, {admin.FullName}!";
        return RedirectToAction("Dashboard", "Admin");
    }

    // GET: /Auth/StudentLogin
    public IActionResult StudentLogin()
    {
        return View();
    }

    // POST: /Auth/StudentLogin
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> StudentLogin(StudentLoginViewModel model)
    {
        if (!ModelState.IsValid)
            return View(model);

        var student = await _authService.LoginStudentAsync(model.RegNo, model.Password);
        if (student == null)
        {
            TempData["Error"] = "Invalid registration number or password.";
            return View(model);
        }

        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, student.RegNo),
            new Claim(ClaimTypes.Name, student.Name),
            new Claim(ClaimTypes.Email, student.Email),
            new Claim(ClaimTypes.Role, "Student"),
            new Claim("RoomId", student.RoomId?.ToString() ?? "")
        };

        var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
        var principal = new ClaimsPrincipal(identity);

        await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);
        TempData["Success"] = $"Welcome back, {student.Name}!";
        return RedirectToAction("Dashboard", "Student");
    }

    // GET: /Auth/Register
    public IActionResult Register()
    {
        return View();
    }

    // POST: /Auth/Register
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Register(StudentRegisterViewModel model)
    {
        if (!ModelState.IsValid)
            return View(model);

        try
        {
            var student = await _authService.RegisterStudentAsync(model.Name, model.Email, model.Phone, model.Password);
            TempData["Success"] = $"Registration successful! Your Registration Number is {student.RegNo}. Please save it for login.";
            TempData["RegNo"] = student.RegNo;
            return RedirectToAction("StudentLogin");
        }
        catch (InvalidOperationException ex)
        {
            TempData["Error"] = ex.Message;
            return View(model);
        }
    }

    // GET: /Auth/Logout
    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        TempData["Success"] = "You have been logged out successfully.";
        return RedirectToAction("Index", "Home");
    }

    // GET: /Auth/AccessDenied
    public IActionResult AccessDenied()
    {
        return View();
    }
}
