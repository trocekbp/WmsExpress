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

            //Current selected category id
            ViewData["CurrentID"] = currentID;
            return View(model);
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

    }
}

