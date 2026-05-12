using Microsoft.AspNetCore.Mvc;

namespace Duvar.Controllers;

public class AboutController : Controller
{
    public IActionResult Index() => View();
}
