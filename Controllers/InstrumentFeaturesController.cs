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
    public class InstrumentFeaturesController : Controller
    {
        private readonly WmsCoreContext _context;

        public InstrumentFeaturesController(WmsCoreContext context)
        {
            _context = context;
        }

        // GET: InstrumentFeatures
        public async Task<IActionResult> Index()
        {
            var WmsCoreContext = _context.InstrumentFeature.Include(i => i.FeatureDefinition).Include(i => i.Instrument);
            return View(await WmsCoreContext.ToListAsync());
        }

        // GET: InstrumentFeatures/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var instrumentFeature = await _context.InstrumentFeature
                .Include(i => i.FeatureDefinition)
                .Include(i => i.Instrument)
                .FirstOrDefaultAsync(m => m.InstrumentFeatureId == id);
            if (instrumentFeature == null)
            {
                return NotFound();
            }

            return View(instrumentFeature);
        }

        // GET: InstrumentFeatures/Create
        public IActionResult Create()
        {
            ViewData["FeatureDefinitionId"] = new SelectList(_context.FeatureDefinition, "FeatureDefinitionId", "FeatureDefinitionId");
            ViewData["InstrumentId"] = new SelectList(_context.Instrument, "InstrumentId", "InstrumentId");
            return View();
        }

        // POST: InstrumentFeatures/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("InstrumentFeatureId,InstrumentId,FeatureDefinitionId,Value")] InstrumentFeature instrumentFeature)
        {
            if (ModelState.IsValid)
            {
                _context.Add(instrumentFeature);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["FeatureDefinitionId"] = new SelectList(_context.FeatureDefinition, "FeatureDefinitionId", "FeatureDefinitionId", instrumentFeature.FeatureDefinitionId);
            ViewData["InstrumentId"] = new SelectList(_context.Instrument, "InstrumentId", "InstrumentId", instrumentFeature.InstrumentId);
            return View(instrumentFeature);
        }

        // GET: InstrumentFeatures/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var instrumentFeature = await _context.InstrumentFeature.FindAsync(id);
            if (instrumentFeature == null)
            {
                return NotFound();
            }
            ViewData["FeatureDefinitionId"] = new SelectList(_context.FeatureDefinition, "FeatureDefinitionId", "FeatureDefinitionId", instrumentFeature.FeatureDefinitionId);
            ViewData["InstrumentId"] = new SelectList(_context.Instrument, "InstrumentId", "InstrumentId", instrumentFeature.InstrumentId);
            return View(instrumentFeature);
        }

        // POST: InstrumentFeatures/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("InstrumentFeatureId,InstrumentId,FeatureDefinitionId,Value")] InstrumentFeature instrumentFeature)
        {
            if (id != instrumentFeature.InstrumentFeatureId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(instrumentFeature);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!InstrumentFeatureExists(instrumentFeature.InstrumentFeatureId))
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
            ViewData["FeatureDefinitionId"] = new SelectList(_context.FeatureDefinition, "FeatureDefinitionId", "FeatureDefinitionId", instrumentFeature.FeatureDefinitionId);
            ViewData["InstrumentId"] = new SelectList(_context.Instrument, "InstrumentId", "InstrumentId", instrumentFeature.InstrumentId);
            return View(instrumentFeature);
        }

        // GET: InstrumentFeatures/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var instrumentFeature = await _context.InstrumentFeature
                .Include(i => i.FeatureDefinition)
                .Include(i => i.Instrument)
                .FirstOrDefaultAsync(m => m.InstrumentFeatureId == id);
            if (instrumentFeature == null)
            {
                return NotFound();
            }

            return View(instrumentFeature);
        }

        // POST: InstrumentFeatures/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var instrumentFeature = await _context.InstrumentFeature.FindAsync(id);
            if (instrumentFeature != null)
            {
                _context.InstrumentFeature.Remove(instrumentFeature);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool InstrumentFeatureExists(int id)
        {
            return _context.InstrumentFeature.Any(e => e.InstrumentFeatureId == id);
        }
    }
}
