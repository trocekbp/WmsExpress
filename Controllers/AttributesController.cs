using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WmsCore.Data;
using WmsCore.Models;

namespace WmsCore.Controllers
{
    public class AttributesController : BaseController
    {
        private readonly WmsCoreContext _context;
        public AttributesController(WmsCoreContext context) {
            _context = context;
        }
        public async Task<IActionResult> Index(int? currentID)
        {

            var model = await _context.Category
           .Include(c => c.AtrDefinitions)
               .ThenInclude(d => d.Attributes)
           .ToListAsync();

            //Odfiltrowanie tylko unikalnych wartości
            foreach (var category in model)
            {
                foreach (var definition in category.AtrDefinitions)
                {
                    definition.Attributes = definition.Attributes.DistinctBy(a => a.Value)
                        .ToList();
                }
            }


            //Current selected category id
            ViewData["CurrentID"] = currentID;
            return View(model);
        }
        public IActionResult Create(int? currentID)
        {
            if (currentID == null) {
                return NotFound();
            }
            //przekazanie id kategorii do widoku
            var model = new AtrDefinition()
            {
                CategoryId = currentID.Value
            };
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(AtrDefinition atrDefinition)
        {
            if (ModelState.IsValid)
            {
                _context.Add(atrDefinition);
                await _context.SaveChangesAsync();


                return RedirectToAction("Index", new { currentID = atrDefinition.CategoryId });
            }
            return View(atrDefinition);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) {
                return NotFound();
            }

            var model = await _context.AtrDefinition
                .Include(i => i.Category)
                .FirstOrDefaultAsync(i => i.AtrDefinitionId == id);

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, AtrDefinition atrDefinition)
        {
            if (id != atrDefinition.AtrDefinitionId)
            {
                return NotFound();
            }
            if (ModelState.IsValid) {
                _context.Update(atrDefinition);
                await _context.SaveChangesAsync();


                return RedirectToAction("Index", new { currentID = atrDefinition.CategoryId});
            }
            return View(atrDefinition);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var model = await _context.AtrDefinition.FindAsync(id);
            if (model == null)
            {
                return NotFound();
            }

            return View(model);
        }

        // POST: Attributes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var definition = await _context.AtrDefinition.FindAsync(id);
            if (definition != null)
            {
                _context.AtrDefinition.Remove(definition);
            }
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (Exception ex) {

                NotifyError(ex.Message + "\n Inner Exception: \n" + ex.InnerException.ToString());

                return View(definition);
            }
            return RedirectToAction(nameof(Index));
        }

    }
}

