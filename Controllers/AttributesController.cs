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
                .ToListAsync();

            //Current selected category id
            ViewData["CurrentID"] = currentID;
            return View(model);
        }
    }
}
