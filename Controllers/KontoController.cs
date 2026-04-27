using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using WebApplication1.Models;

namespace WebApplication1.Controllers
{
    public class KontoController : Controller
    {
        private readonly BibliotekaContext _context;

        public KontoController(BibliotekaContext context)
        {
            _context = context;
        }

        private string HashujHaslo(string haslo)
        {
            using (var sha256 = SHA256.Create())
            {
                var bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(haslo));
                var builder = new StringBuilder();
                foreach (var b in bytes) builder.Append(b.ToString("x2"));
                return builder.ToString();
            }
        }

        [HttpGet]
        public IActionResult Register() => View();

        [HttpPost]
        public async Task<IActionResult> Register(Uzytkownik uzytkownik, string Haslo)
        {
            if (_context.Uzytkownicy.Any(u => u.Login == uzytkownik.Login))
            {
                ModelState.AddModelError("", "Taki login już istnieje.");
                return View(uzytkownik);
            }

            uzytkownik.HasloHash = HashujHaslo(Haslo);
            _context.Uzytkownicy.Add(uzytkownik);
            await _context.SaveChangesAsync();

            return RedirectToAction("Login");
        }

        [HttpGet]
        public IActionResult Login() => View();

        [HttpPost]
        public async Task<IActionResult> Login(string login, string haslo)
        {
            var podanyHash = HashujHaslo(haslo);
            var uzytkownik = _context.Uzytkownicy.FirstOrDefault(u => u.Login == login && u.HasloHash == podanyHash);

            if (uzytkownik != null)
            {
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, uzytkownik.Login),
                    new Claim(ClaimTypes.NameIdentifier, uzytkownik.Id.ToString())
                };

                var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                var principal = new ClaimsPrincipal(identity);

                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);
                return RedirectToAction("Index", "Home");
            }

            ModelState.AddModelError("", "Błędny login lub hasło.");
            return View();
        }

        [HttpGet]
        public IActionResult BrakDostepu()
        {
            return View();
        }

        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Index", "Home");
        }
    }
}