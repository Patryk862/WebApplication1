using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using WebApplication1.Models;
using System.Security.Claims; // <-- NIEZBĘDNE DO POBRANIA ID UŻYTKOWNIKA

namespace WebApplication1
{
    [Authorize]
    public class KsiazkiController : Controller
    {
        private readonly BibliotekaContext _context;

        public KsiazkiController(BibliotekaContext context)
        {
            _context = context;
        }

        [AllowAnonymous]
        public async Task<IActionResult> Index(string searchString)
        {
            var ksiazki = _context.Ksiazki.Include(k => k.Wydawnictwo).AsQueryable();

            if (!String.IsNullOrEmpty(searchString))
            {
                ksiazki = ksiazki.Where(k => k.Tytul.Contains(searchString));
            }

            ViewData["CurrentFilter"] = searchString;
            return View(await ksiazki.ToListAsync());
        }

        [AllowAnonymous]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var ksiazka = await _context.Ksiazki
                .Include(k => k.Wydawnictwo)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (ksiazka == null) return NotFound();

            return View(ksiazka);
        }

        public IActionResult Create()
        {
            ViewData["WydawnictwoId"] = new SelectList(_context.Wydawnictwa, "Id", "Nazwa");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Tytul,WydawnictwoId")] Ksiazka ksiazka)
        {
            // 1. ZAPISUJEMY WŁAŚCICIELA KSIĄŻKI
            ksiazka.UserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            
            ModelState.Remove("Wydawnictwo");
            ModelState.Remove("KsiazkaAutorzy");
            ModelState.Remove("UserId"); // Ignorujemy walidację tego pola z formularza

            if (ModelState.IsValid)
            {
                _context.Add(ksiazka);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["WydawnictwoId"] = new SelectList(_context.Wydawnictwa, "Id", "Nazwa", ksiazka.WydawnictwoId);
            return View(ksiazka);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var ksiazka = await _context.Ksiazki.FindAsync(id);
            if (ksiazka == null) return NotFound();

            // 2. OCHRONA: CZY TO TWOJA KSIĄŻKA?
            if (ksiazka.UserId != User.FindFirstValue(ClaimTypes.NameIdentifier))
            {
                return Forbid(); // Zwraca błąd odmowy dostępu
            }

            ViewData["WydawnictwoId"] = new SelectList(_context.Wydawnictwa, "Id", "Nazwa", ksiazka.WydawnictwoId);
            return View(ksiazka);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Tytul,WydawnictwoId,UserId")] Ksiazka ksiazka)
        {
            if (id != ksiazka.Id) return NotFound();

            // Zabezpieczenie przed atakiem (nie pozwalamy zmienić właściciela w ukryciu)
            if (ksiazka.UserId != User.FindFirstValue(ClaimTypes.NameIdentifier))
            {
                return Forbid();
            }

            ModelState.Remove("Wydawnictwo");
            ModelState.Remove("KsiazkaAutorzy");
            ModelState.Remove("UserId");

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(ksiazka);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!KsiazkaExists(ksiazka.Id)) return NotFound();
                    else throw;
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["WydawnictwoId"] = new SelectList(_context.Wydawnictwa, "Id", "Nazwa", ksiazka.WydawnictwoId);
            return View(ksiazka);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var ksiazka = await _context.Ksiazki
                .Include(k => k.Wydawnictwo)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (ksiazka == null) return NotFound();

            // 3. OCHRONA PRZED USUNIĘCIEM CUDZEJ KSIĄŻKI
            if (ksiazka.UserId != User.FindFirstValue(ClaimTypes.NameIdentifier))
            {
                return Forbid();
            }

            return View(ksiazka);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var ksiazka = await _context.Ksiazki.FindAsync(id);
            
            // 4. OSTATNIA LINIA OBRONY PRZED FAKTYCZNYM USUNIĘCIEM
            if (ksiazka != null && ksiazka.UserId == User.FindFirstValue(ClaimTypes.NameIdentifier))
            {
                _context.Ksiazki.Remove(ksiazka);
                await _context.SaveChangesAsync();
            }
            
            return RedirectToAction(nameof(Index));
        }

        private bool KsiazkaExists(int id)
        {
            return _context.Ksiazki.Any(e => e.Id == id);
        }
    }
}