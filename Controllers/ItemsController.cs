using System;
using System.Collections.Generic;
using System.Diagnostics.Metrics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using WmsCore.Data;
using WmsCore.Models;

namespace Music_Store_Warehouse_App.Views
{
    public class ItemsController : Controller
    {
        private readonly WmsCoreContext _context;

        public ItemsController(WmsCoreContext context)
        {
            _context = context;
        }

        // GET: Items
        // GET: Items
        public async Task<IActionResult> Index(
            string sortOrder,
            string currentFilter,
            string searchString,
            int? pageNumber,
            int? categoryId)
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


            ViewData["CategoryList"] = new SelectList(categories, "CategoryId", "Name", categoryId);
            ViewData["CurrentCategory"] = categoryId; // by wiedzieć, która opcja ma być selected


            IQueryable<Item> items = _context.Item
                                                .Include(i => i.Category)
                                                .Include(i => i.ItemInventory);

            // --- Filtrowanie po wyszukiwanej frazie ---
            if (!String.IsNullOrEmpty(searchString))
            {
                items = items.Where(s =>
                    s.Name.Contains(searchString) || s.Acronym.Contains(searchString));
            }

            // --- Filtrowanie po kategorii, jeśli użytkownik wybrał categoryId ---
            if (categoryId.HasValue)
            {
                items = items.Where(i => i.CategoryId == categoryId.Value);
            }


            switch (sortOrder)
            {
                case "name_desc":
                    items = items.OrderByDescending(s => s.Name);
                    break;
                case "Price":
                    items = items.OrderBy(s => s.Price);
                    break;
                case "price_desc":
                    items = items.OrderByDescending(s => s.Price);
                    break;
                default:
                    items = items.OrderBy(s => s.Name);
                    break;
            }

            int pageSize = 10;
            return View(await PaginatedList<Item>.CreateAsync(items.AsNoTracking(), pageNumber ?? 1, pageSize));
        }

        // GET: itemss/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var items = await _context.Item
                .Include(i => i.Category)
                .Include(i => i.ItemInventory)
                .Include(i => i.Attributes)
                    .ThenInclude(ifeat => ifeat.AtrDefinition) // ThenInclude - jeszcze dołączamy definicje cech
                .FirstOrDefaultAsync(m => m.ItemId == id);

            if (items == null)
            {
                return NotFound();
            }

            return View(items);
        }

      

        // GET: Items/Create
        public IActionResult Create()
        {
            ViewData["CategoryId"] = new SelectList(_context.Category, "CategoryId", "CategoryId");
            return View();
        }

        // POST: Items/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ItemId,Code,Acronym,Name,Price,Description,EAN,CategoryId")] Item item, string action,  // "ShowAttributess" lub "SaveInstrument"
            [FromForm] IList<WmsCore.Models.Attribute> Attributes)
        {
            // Zawsze przygotowujemy ViewBag dropdownów, bo widok ich potrzebuje
            PrepareViewBags();

            // Jeżeli użytkownik wcisnął „Pokaż cechy”
            if (action == "ShowAttributes")
            {
                // Upewnijmy się, że wybrano kategorię
                if (item.CategoryId == 0)
                {
                    ModelState.AddModelError(nameof(item.CategoryId), "Kategoria jest wymagana, aby wyświetlić cechy.");
                }
                else
                {
                    // Jeżeli jest wartość CategoryId, ładujemy od razu cechy dla danej grupy towarów
                    var cat = _context.Category.Find(item.CategoryId);
                    if (cat != null)
                    {
                        var type = cat.Name;
                        ViewBag.AttributesDefinitions = _context.AtrDefinition
                            .Where(a => a.AttributeGroup.Name == type)
                            .ToList();
                    }
                }

                // Zwracamy widok z aktualnym modelem, nawet jeśli np. Name lub Price są puste.

                return View(item);
            }

            // Jeżeli użytkownik wcisnął „Zapisz instrument”
            else if (action == "SaveInstrument")
            {
                // Przypisz z POST-a listę cech (może być pusta, jeżeli nie było tabeli)
                item.Attributes = Attributes;

                // Teraz wykonujemy normalną walidację całego modelu
                if (!ModelState.IsValid)
                {
                    // Aby tabela cech nie zniknęła, musimy znów załadować AttributesDefinitions
                    if (item.CategoryId != 0)
                    {
                        var cat = _context.Category.Find(item.CategoryId);
                        var type = cat.Name;
                        ViewBag.AttributesDefinitions = _context.AtrDefinition
                            .Where(a => a.AttributeGroup.Name == type)
                            .ToList();
                    }
                    return View(item);
                }

                // Jeśli ModelState jest OK – zapisujemy do bazy:
                _context.Add(item);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            // Jeśli action nie został przekazany (lub ma nietypową wartość), po prostu wyświetlamy formularz
            return View(item);
        }


        private void PrepareViewBags()
        {
            ViewBag.SupplierList = new SelectList(
                _context.Contractor, "ContractorId", "Name");

            ViewBag.CategoryList = new SelectList(
                _context.Category, "CategoryId", "Name");
        }


        // GET: Items/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var item = await _context.Item
                .Include(i => i.Category)
                .Include(i => i.Attributes)
                    .ThenInclude(atr => atr.AtrDefinition)
                .FirstOrDefaultAsync(i => i.ItemId == id);

            if (item == null)
            {
                return NotFound();
            }
            // ViewData["CategoryId"] = new SelectList(_context.Category, "CategoryId", "CategoryId", item.CategoryId);
            PrepareViewBags();
            return View(item);
        }

        // POST: Items/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ItemId,Code,Acronym,Name,Price,Description,EAN,CategoryId")] Item item)
        {
            if (id != item.ItemId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(item);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ItemExists(item.ItemId))
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
            ViewData["CategoryId"] = new SelectList(_context.Category, "CategoryId", "CategoryId", item.CategoryId);
            return View(item);
        }

        // GET: Items/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var item = await _context.Item
                .Include(i => i.Category)
                .FirstOrDefaultAsync(m => m.ItemId == id);
            if (item == null)
            {
                return NotFound();
            }

            return View(item);
        }

        // POST: Items/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var item = await _context.Item.FindAsync(id);
            if (item != null)
            {
                _context.Item.Remove(item);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ItemExists(int id)
        {
            return _context.Item.Any(e => e.ItemId == id);
        }
    }
}
