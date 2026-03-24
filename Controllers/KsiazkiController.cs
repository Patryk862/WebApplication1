using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using WebApplication1.Models;

namespace WebApplication1
{
    public class KsiazkiController : Controller
    {
        private readonly BibliotekaContext _context;

        public KsiazkiController(BibliotekaContext context)
        {
            _context = context;
        }

        // GET: Ksiazki
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

        // GET: Ksiazki/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var ksiazka = await _context.Ksiazki
                .Include(k => k.Wydawnictwo)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (ksiazka == null)
            {
                return NotFound();
            }

            return View(ksiazka);
        }

        // GET: Ksiazki/Create
        public IActionResult Create()
        {
            ViewData["WydawnictwoId"] = new SelectList(_context.Wydawnictwa, "Id", "Nazwa");
            return View();
        }

        // POST: Ksiazki/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Tytul,WydawnictwoId")] Ksiazka ksiazka)
        {
            ModelState.Remove("Wydawnictwo");
            ModelState.Remove("KsiazkaAutorzy");
            if (ModelState.IsValid)
            {
                _context.Add(ksiazka);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["WydawnictwoId"] = new SelectList(_context.Wydawnictwa, "Id", "Nazwa", ksiazka.WydawnictwoId);
            return View(ksiazka);
        }

        // GET: Ksiazki/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var ksiazka = await _context.Ksiazki.FindAsync(id);
            if (ksiazka == null)
            {
                return NotFound();
            }
            ViewData["WydawnictwoId"] = new SelectList(_context.Wydawnictwa, "Id", "Nazwa", ksiazka.WydawnictwoId);
            return View(ksiazka);
        }

        // POST: Ksiazki/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Tytul,WydawnictwoId")] Ksiazka ksiazka)
        {
            if (id != ksiazka.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(ksiazka);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!KsiazkaExists(ksiazka.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["WydawnictwoId"] = new SelectList(_context.Wydawnictwa, "Id", "Nazwa", ksiazka.WydawnictwoId);
            return View(ksiazka);
        }

        // GET: Ksiazki/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var ksiazka = await _context.Ksiazki
                .Include(k => k.Wydawnictwo)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (ksiazka == null)
            {
                return NotFound();
            }

            return View(ksiazka);
        }

        // POST: Ksiazki/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var ksiazka = await _context.Ksiazki.FindAsync(id);
            if (ksiazka != null)
            {
                _context.Ksiazki.Remove(ksiazka);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool KsiazkaExists(int id)
        {
            return _context.Ksiazki.Any(e => e.Id == id);
        }
    }
}
