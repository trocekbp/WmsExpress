using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using WmsCore.Data;
using WmsCore.Models;

namespace WmsCore.Controllers
{
    public class CategoriesController : BaseController
    {
        private readonly WmsCoreContext _context;

        public CategoriesController(WmsCoreContext context)
        {
            _context = context;
        }

        // GET: Categories
        public async Task<IActionResult> Index()
        {
            return View(await _context.Category.ToListAsync());
        }


        // GET: Categories/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Categories/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("CategoryId,Name")] Category category)
        {
            if (ModelState.IsValid)
            {
                _context.Add(category);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(category);
        }

        // GET: Categories/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var category = await _context.Category.FindAsync(id);
            if (category == null)
            {
                return NotFound();
            }
            return View(category);
        }

        // POST: Categories/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("CategoryId,Name")] Category category)
        {
            if (id != category.CategoryId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(category);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CategoryExists(category.CategoryId))
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
            return View(category);
        }

        // GET: Categories/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var category = await _context.Category
                .FirstOrDefaultAsync(m => m.CategoryId == id);
            if (category == null)
            {
                return NotFound();
            }

            if (category.Name.Equals("Inne"))
            {
                NotifyError("Nie można usunąć domyślnej kategorii.");
                return RedirectToAction(nameof(Index));
            }

            return View(category);
        }

        // POST: Categories/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var category = await _context.Category
                .Include(m => m.Articles)
                .FirstOrDefaultAsync(c => c.CategoryId == id);

            if (category != null)
            {
                if (category.Name.Equals("Inne"))
                {
                    NotifyError("Nie można usunąć domyślnej kategorii.");
                    return RedirectToAction(nameof(Index));
                }
                var otherCat = await _context.Category.FirstOrDefaultAsync(i => i.Name == "Inne");
                if (otherCat == null) {
                    NotifyError("Brak domyślnej kategorii w systemie, skontaktuj się z administratorem systemu.");
                    return RedirectToAction(nameof(Index));
                }
                //Zamiast wstawiać null w artykułach podajemy kategorię Inne
                foreach (var article in category.Articles)
                {
                    article.CategoryId = otherCat.CategoryId;
                }
                    _context.Category.Remove(category);
            }

            await _context.SaveChangesAsync();
            NotifySuccess("Usunięto kategorię, powiązane artykuły zostały przypisane do kategorii \"Inne\" ");
            return RedirectToAction(nameof(Index));
        }

        private bool CategoryExists(int id)
        {
            return _context.Category.Any(e => e.CategoryId == id);
        }
    }
}
