using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using WmsCore.Data;
using WmsCore.Models;
using WmsCore.ViewModels;

namespace Music_Store_Warehouse_App.Controllers
{
    public class DocumentItemsController : Controller
    {
        private readonly WmsCoreContext _context;

        public DocumentItemsController(WmsCoreContext context)
        {
            _context = context;
        }

        // GET: DocumentItems
        public async Task<IActionResult> Index(
            int documentId,
            string sortOrder,
            string currentFilter,
            string searchString,
            int? pageNumber,
            int? categoryId,
            string? documentType)
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
            ViewBag.DocumentType = documentType;
            ViewBag.DocumentId = documentId;
            return View(await PaginatedList<Item>.CreateAsync(items.AsNoTracking(), pageNumber ?? 1, pageSize));
        }

        // GET: DocumentItems/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var documentItem = await _context.DocumentItem
                .Include(d => d.Document)
                .Include(d => d.Item)
                .FirstOrDefaultAsync(m => m.DocumentItemId == id);
            if (documentItem == null)
            {
                return NotFound();
            }

            return View(documentItem);
        }

        // GET: DocumentItems/Create
        public IActionResult Create(int documentId, List<int> selectedIds)
        {
            if (selectedIds == null || !selectedIds.Any())
                return RedirectToAction("Index", new { documentId }); // brak wybranych pozycji
            var itemsFromDb = _context.Item
                .Where(i => selectedIds.Contains(i.ItemId))
                .Include(i => i.Category)
                .ToList();

            var documentItems = itemsFromDb.Select(i => new DocumentItem
            {
                DocumentId = documentId,
                ItemId = i.ItemId,
                Item = i,
            }).ToList();

            return View(documentItems);
        }

        // POST: DocumentItems/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(List<DocumentItem> documentItems)
        {
            if (ModelState.IsValid)
            {
                _context.DocumentItem.AddRange(documentItems);
                await _context.SaveChangesAsync();
                int docId = documentItems.FirstOrDefault().DocumentId;
                return RedirectToAction("Details", "Documents", new {id =  docId});
            }

            return View(documentItems);
        }

        // GET: DocumentItems/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var documentItem = await _context.DocumentItem.FindAsync(id);
            if (documentItem == null)
            {
                return NotFound();
            }
            ViewData["DocumentId"] = new SelectList(_context.Document, "DocumentId", "DocumentId", documentItem.DocumentId);
            ViewData["ItemId"] = new SelectList(_context.Item, "ItemId", "Code", documentItem.ItemId);
            return View(documentItem);
        }

        // POST: DocumentItems/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("DocumentItemId,DocumentId,ItemId,Quantity,UnitOfMeasure")] DocumentItem documentItem)
        {
            if (id != documentItem.DocumentItemId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(documentItem);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!DocumentItemExists(documentItem.DocumentItemId))
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
            ViewData["DocumentId"] = new SelectList(_context.Document, "DocumentId", "DocumentId", documentItem.DocumentId);
            ViewData["ItemId"] = new SelectList(_context.Item, "ItemId", "Code", documentItem.ItemId);
            return View(documentItem);
        }

        // GET: DocumentItems/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var documentItem = await _context.DocumentItem
                .Include(d => d.Document)
                .Include(d => d.Item)
                .FirstOrDefaultAsync(m => m.DocumentItemId == id);
            if (documentItem == null)
            {
                return NotFound();
            }

            return View(documentItem);
        }

        // POST: DocumentItems/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var documentItem = await _context.DocumentItem.FindAsync(id);
            if (documentItem != null)
            {
                _context.DocumentItem.Remove(documentItem);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool DocumentItemExists(int id)
        {
            return _context.DocumentItem.Any(e => e.DocumentItemId == id);
        }
    }
}
