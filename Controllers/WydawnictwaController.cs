using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApplication1.Models;
using System.Security.Claims; // <-- TO JEST POTRZEBNE DO POBRANIA ID UŻYTKOWNIKA

namespace WebApplication1
{
    [Authorize]
    public class WydawnictwaController : Controller
    {
        private readonly BibliotekaContext _context;

        public WydawnictwaController(BibliotekaContext context)
        {
            _context = context;
        }

        [AllowAnonymous]
        public async Task<IActionResult> Index()
        {
            return View(await _context.Wydawnictwa.ToListAsync());
        }

        [AllowAnonymous]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();
            var wydawnictwo = await _context.Wydawnictwa.FirstOrDefaultAsync(m => m.Id == id);
            if (wydawnictwo == null) return NotFound();
            return View(wydawnictwo);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Nazwa")] Wydawnictwo wydawnictwo)
        {
            // 1. ZAPISUJEMY KTO TWORZY WPIS
            wydawnictwo.UserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            
            // 2. Usuwamy to z walidacji, żeby serwer nie krzyczał, że pole było puste w formularzu
            ModelState.Remove("UserId");

            if (ModelState.IsValid)
            {
                _context.Add(wydawnictwo);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(wydawnictwo);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var wydawnictwo = await _context.Wydawnictwa.FindAsync(id);
            if (wydawnictwo == null) return NotFound();

            // 3. OCHRONA: CZY TO JEST TWÓJ WPIS?
            if (wydawnictwo.UserId != User.FindFirstValue(ClaimTypes.NameIdentifier))
            {
                return Forbid(); // Odcięcie dostępu (Zwraca oficjalny błąd braku uprawnień)
            }

            return View(wydawnictwo);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Nazwa,UserId")] Wydawnictwo wydawnictwo)
        {
            if (id != wydawnictwo.Id) return NotFound();

            // Zabezpieczenie przed atakiem (nie pozwalamy zmienić właściciela)
            if (wydawnictwo.UserId != User.FindFirstValue(ClaimTypes.NameIdentifier))
            {
                return Forbid();
            }

            ModelState.Remove("UserId");

            if (ModelState.IsValid)
            {
                _context.Update(wydawnictwo);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(wydawnictwo);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var wydawnictwo = await _context.Wydawnictwa.FirstOrDefaultAsync(m => m.Id == id);
            if (wydawnictwo == null) return NotFound();

            // OCHRONA PRZED USUNIĘCIEM
            if (wydawnictwo.UserId != User.FindFirstValue(ClaimTypes.NameIdentifier))
            {
                return Forbid();
            }

            return View(wydawnictwo);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var wydawnictwo = await _context.Wydawnictwa.FindAsync(id);
            
            // OSTATNIA LINIA OBRONY PRZED USUNIĘCIEM
            if (wydawnictwo != null && wydawnictwo.UserId == User.FindFirstValue(ClaimTypes.NameIdentifier))
            {
                _context.Wydawnictwa.Remove(wydawnictwo);
                await _context.SaveChangesAsync();
            }
            
            return RedirectToAction(nameof(Index));
        }
    }
}