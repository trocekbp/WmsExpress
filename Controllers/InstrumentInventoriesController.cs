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
    public class InstrumentInventoriesController : Controller
    {
        private readonly WmsCoreContext _context;

        public InstrumentInventoriesController(WmsCoreContext context)
        {
            _context = context;
        }

        // GET: InstrumentInventories

        //Klasa ta różnie się tym że operujemy tlyko na instrumentach które są na magazynie poprzez .Where(i => i.inventory != null)
        public async Task<IActionResult> Index(

            string sortOrder,
            string currentFilter,
            string searchString,
            int? pageNumber,
            int? categoryId,
            int? supplierId)
        {
                ViewData["CurrentSort"] = sortOrder;
                ViewData["NameSortParm"] = String.IsNullOrEmpty(sortOrder) ? "name_desc" : "";
                ViewData["PriceSortParm"] = sortOrder == "Price" ? "price_desc" : "Price";
                ViewData["CurrentFilter"] = searchString;

                if (searchString != null)
                {
                    pageNumber = 1;
                }
                else
                {
                    searchString = currentFilter;
                }

                var categories = await _context.Category
                               .OrderBy(c => c.Name)
                               .ToListAsync();

                var suppliers = await _context.Supplier
                            .OrderBy(s => s.Name)
                            .ToListAsync();

                ViewData["CategoryList"] = new SelectList(categories, "CategoryId", "Name", categoryId);
                ViewData["CurrentCategory"] = categoryId; // by wiedzieć, która opcja ma być selected

                ViewData["SupplierList"] = new SelectList(suppliers, "SupplierId", "Name", supplierId);
                ViewData["CurrentSupplier"] = supplierId; // by wiedzieć, która opcja ma być selected

            IQueryable<Instrument> instruments = _context.Instrument // instruments =  INSTRUMENTS IN INVENTORY !!!
                                                    .Include(i => i.Category)
                                                    .Include(i => i.Inventory)
                                                    .Include(i => i.Supplier)
                                                    .Where(i => i.Inventory != null); //Czyli żeby było prościej pobieramy tylko instrumenty które są na stanie

                // --- Filtrowanie po wyszukiwanej frazie ---
                if (!String.IsNullOrEmpty(searchString))
                {
                    instruments = instruments.Where(s =>
                        s.Name.Contains(searchString) ||
                        s.Supplier.Name.Contains(searchString));
                }

                // --- Filtrowanie po kategorii, jeśli użytkownik wybrał categoryId ---
                if (categoryId.HasValue)
                {
                    instruments = instruments.Where(i => i.CategoryId == categoryId.Value);
                }

                if (supplierId.HasValue)
                {
                    instruments = instruments.Where(i => i.SupplierId == supplierId.Value);
                }

            switch (sortOrder)
                {
                    case "name_desc":
                        instruments = instruments.OrderByDescending(s => s.Name);
                        break;
                    case "Price":
                        instruments = instruments.OrderBy(s => s.Price);
                        break;
                    case "price_desc":
                        instruments = instruments.OrderByDescending(s => s.Price);
                        break;
                    default:
                        instruments = instruments.OrderBy(s => s.Name);
                        break;
                }

                int pageSize = 10;
                return View(await PaginatedList<Instrument>.CreateAsync(instruments.AsNoTracking(), pageNumber ?? 1, pageSize));
        }

        // GET: InstrumentInventories/Details/5
        // GET: Instruments/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var instrument = await _context.Instrument
                .Include(i => i.Category)
                .Include(i => i.Supplier)
                .Include(i => i.Inventory)
                .Include(i => i.InstrumentFeatures)
                    .ThenInclude(ifeat => ifeat.FeatureDefinition) // ThenInclude - jeszcze dołączamy definicje cech
                .Where(i => i.Inventory != null) //zabezpieczenie
                .FirstOrDefaultAsync(m => m.InstrumentId == id);

            if (instrument == null)
            {
                return NotFound();
            }

            return View(instrument);
        }

        // GET: InstrumentInventories/Create
        public IActionResult Create()
        {
            ViewBag.SupplierList = new SelectList(_context.Supplier, "SupplierId", "Name");
            return View();
        }

        // POST: InstrumentInventories/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(
     int? supplierId,
     string action,
     List<int> selectedInstrumentIds)
        {
            ViewBag.SupplierList = new SelectList(_context.Supplier, "SupplierId", "Name", supplierId);

            if (action == "ShowSupplierInstruments")
            {
                if (supplierId == null)
                {
                    ModelState.AddModelError("supplierId", "Musisz wybrać dostawcę.");
                }
                else
                {
                    ViewBag.Instruments = _context.Instrument
                        .Where(i => i.SupplierId == supplierId)
                        .ToList();

                    // PRZEKAZUJEMY supplierId osobno:
                    ViewBag.SelectedSupplierId = supplierId;
                }
                return View();
            }

            // „Dalej”
            if (selectedInstrumentIds == null || !selectedInstrumentIds.Any())
            {
                ModelState.AddModelError("", "Musisz zaznaczyć przynajmniej jeden instrument.");
                ViewBag.Instruments = _context.Instrument
                    .Where(i => i.SupplierId == supplierId)
                    .ToList();
                ViewBag.SelectedSupplierId = supplierId;
                return View();
            }

            //przekieruj do Documents/Create z parametrami
            return RedirectToAction(
                actionName: "Create",
                controllerName: "Documents",
                routeValues: new
                {
                    supplierId = supplierId,
                    selectedInstrumentIds = selectedInstrumentIds
                }
            );
        }





        // GET: InstrumentInventories/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var instrument = await _context.Instrument
                .Include(i => i.Category)
                .Include(i => i.Supplier)
                .Include(i => i.Inventory)
                .Include(i => i.InstrumentFeatures)
                    .ThenInclude(ifeat => ifeat.FeatureDefinition) // ThenInclude - jeszcze dołączamy definicje cech
                .FirstOrDefaultAsync(m => m.InstrumentId == id);
            if (instrument == null)
            {
                return NotFound();
            }

            ViewBag.SupplierList = new SelectList(
               _context.Supplier, "SupplierId", "Name");

            ViewBag.CategoryList = new SelectList(
                _context.Category, "CategoryId", "Name");

            return View(instrument);
        }

        // POST: Instruments/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("InstrumentId,Name,Price,Description,EAN,SKU,SerialNumber,Inventory,SupplierId,CategoryId,InstrumentFeatures")] Instrument instrument)
        {
            if (id != instrument.InstrumentId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(instrument);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!InstrumentExists(instrument.InstrumentId))
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
            ViewData["CategoryId"] = new SelectList(_context.Category, "CategoryId", "CategoryId", instrument.CategoryId);
            ViewData["SupplierId"] = new SelectList(_context.Supplier, "SupplierId", "SupplierId", instrument.SupplierId);
            return View(instrument);
        }

        // GET: InstrumentInventories/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var instrumentInventory = await _context.InstrumentInventory
                .Include(i => i.Instrument)
                .FirstOrDefaultAsync(m => m.InstrumentId == id);
            if (instrumentInventory == null)
            {
                return NotFound();
            }

            return View(instrumentInventory);
        }

        // POST: InstrumentInventories/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var instrumentInventory = await _context.InstrumentInventory
                                     .Include(i => i.Instrument)
                                    .FirstOrDefaultAsync(m => m.InstrumentId == id); ;
            if (instrumentInventory != null)
            {
                _context.InstrumentInventory.Remove(instrumentInventory);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool InstrumentExists(int id)
        {
            return _context.InstrumentInventory.Any(e => e.InstrumentInventoryId == id);
        }

    }
}
