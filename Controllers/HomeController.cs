using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using WebApplication1.Models;
using Microsoft.AspNetCore.Identity;

namespace WebApplication1.Controllers;

public class HomeController : Controller
{
    public IActionResult Index()
    {
        return View();
    }

    public IActionResult Privacy()
    {
        return View();
    }

    // --- NASZA NOWA METODA WYLOGOWYWANIA ---
    [HttpPost]
    public async Task<IActionResult> Logout([FromServices] SignInManager<IdentityUser> signInManager)
    {
        // 1. Wykonuje fizyczne usunięcie ciasteczka (wylogowanie)
        await signInManager.SignOutAsync();
        
        // 2. Przekierowuje bezpiecznie na stronę główną
        return RedirectToAction("Index", "Home");
    }
    // ---------------------------------------

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}