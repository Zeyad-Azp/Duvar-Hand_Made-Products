using Microsoft.AspNetCore.Mvc;

namespace Duvar.Controllers;

public class ContactController : Controller
{
    public IActionResult Index() => View();

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Send(string name, string email, string message)
    {
        // In production: integrate SMTP or save to DB
        TempData["ContactSuccess"] = "Thank you for reaching out. We'll be in touch within 2 business days.";
        return RedirectToAction(nameof(Index));
    }
}
