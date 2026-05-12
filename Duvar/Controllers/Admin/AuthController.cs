using Duvar.BLL.Services.Abstraction;
using Duvar.BLL.ViewModels.Admin;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Duvar.Controllers.Admin;

public class AuthController : Controller
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    // GET /my-admin/login
    public IActionResult Login()
    {
        if (User.Identity?.IsAuthenticated == true)
            return Redirect("/my-admin/mydashboard");
        return View();
    }

    // POST /my-admin/login
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(LoginViewModel model)
    {
        if (!ModelState.IsValid) return View(model);

        var admin = await _authService.AuthenticateAsync(model.Username, model.Password);

        if (admin == null)
        {
            ModelState.AddModelError(string.Empty, "Invalid username or password.");
            return View(model);
        }

        var claims = new List<Claim>
        {
            new(ClaimTypes.Name, admin.Username),
            new(ClaimTypes.NameIdentifier, admin.Id.ToString()),
            new(ClaimTypes.Role, "Admin")
        };

        var identity  = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
        var principal = new ClaimsPrincipal(identity);

        await HttpContext.SignInAsync(
            CookieAuthenticationDefaults.AuthenticationScheme,
            principal,
            new AuthenticationProperties
            {
                IsPersistent = true,
                ExpiresUtc   = DateTimeOffset.UtcNow.AddHours(8)
            });

        return Redirect("/my-admin/mydashboard");
    }

    // POST /my-admin/logout
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        return Redirect("/my-admin/login");
    }
}
