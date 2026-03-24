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
    public class WydawnictwaController : Controller
    {
        private readonly BibliotekaContext _context;

        public WydawnictwaController(BibliotekaContext context)
        {
            _context = context;
        }

        // GET: Wydawnictwa
        public async Task<IActionResult> Index()
        {
            return View(await _context.Wydawnictwa.ToListAsync());
        }

        // GET: Wydawnictwa/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var wydawnictwo = await _context.Wydawnictwa
                .FirstOrDefaultAsync(m => m.Id == id);
            if (wydawnictwo == null)
            {
                return NotFound();
            }

            return View(wydawnictwo);
        }

        // GET: Wydawnictwa/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Wydawnictwa/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Nazwa")] Wydawnictwo wydawnictwo)
        {
            if (ModelState.IsValid)
            {
                _context.Add(wydawnictwo);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(wydawnictwo);
        }

        // GET: Wydawnictwa/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var wydawnictwo = await _context.Wydawnictwa.FindAsync(id);
            if (wydawnictwo == null)
            {
                return NotFound();
            }
            return View(wydawnictwo);
        }

        // POST: Wydawnictwa/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Nazwa")] Wydawnictwo wydawnictwo)
        {
            if (id != wydawnictwo.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(wydawnictwo);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!WydawnictwoExists(wydawnictwo.Id))
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
            return View(wydawnictwo);
        }

        // GET: Wydawnictwa/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var wydawnictwo = await _context.Wydawnictwa
                .FirstOrDefaultAsync(m => m.Id == id);
            if (wydawnictwo == null)
            {
                return NotFound();
            }

            return View(wydawnictwo);
        }

        // POST: Wydawnictwa/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var wydawnictwo = await _context.Wydawnictwa.FindAsync(id);
            if (wydawnictwo != null)
            {
                _context.Wydawnictwa.Remove(wydawnictwo);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool WydawnictwoExists(int id)
        {
            return _context.Wydawnictwa.Any(e => e.Id == id);
        }
    }
}
