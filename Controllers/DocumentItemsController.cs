using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
           DocumentItemsViewModel vm)
        {
            if (vm.DocumentId == 0){
             return BadRequest("Brak Id dokumentu");
            }

            ViewData["CodeSortParam"] = String.IsNullOrEmpty(vm.SortOrder) ? "code_desc" : "";
            ViewData["NameSortParam"] = vm.SortOrder == "Name" ? "name_desc" : "Name";
            ViewData["PriceSortParam"] = vm.SortOrder == "Price" ? "price_desc" : "Price";

            //obsługa filtrów
            if (vm.SearchString != null)
            {
                vm.PageNumber = 1;
            }


            var categories = await _context.Category
                           .OrderBy(c => c.Name)
                           .ToListAsync();


           vm.CategoryList= new SelectList(categories, "CategoryId", "Name", vm.CategoryId);            


            IQueryable<Item> items = _context.Item
                                                .Include(i => i.Category)
                                                .Include(i => i.ItemInventory);

            // --- Filtrowanie po wyszukiwanej frazie ---
            if (!String.IsNullOrEmpty(vm.SearchString))
            {
                items = items.Where(s =>
                    s.Name.Contains(vm.SearchString) || s.Acronym.Contains(vm.SearchString));
            }

            // --- Filtrowanie po kategorii, jeśli użytkownik wybrał categoryId ---
            if (vm.CategoryId.HasValue)
            {
                items = items.Where(i => i.CategoryId == vm.CategoryId.Value);
            }


            switch (vm.SortOrder)
            {
                case "code_desc":
                    items = items.OrderByDescending(s => s.Code);
                    break;
                case "Name":
                    items = items.OrderBy(s => s.Name);
                    break;
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
                    items = items.OrderBy(s => s.Code);
                    break;
            }

            int pageSize = 15;

            var paginatedList = await PaginatedList<Item>.CreateAsync(items.AsNoTracking(), vm.PageNumber ?? 1, pageSize);
            vm.Items = paginatedList;
            return View(vm);
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
        public IActionResult Create(DocumentItemsViewModel vm)
        {
            if (vm.SelectedItems == null || !vm.SelectedItems.Any())
            {
                TempData["ErrorMessage"] = "Musisz zaznaczyć przynajmniej jedną pozycję.";
                return RedirectToAction("Index", new
                {
                    DocumentId = vm.DocumentId,
                    DocumentType = vm.DocumentType
                }); // brak wybranych pozycji
            }
            var itemsFromDb = _context.Item
                .Where(i => vm.SelectedItems.Contains(i.ItemId))
                .Include(i => i.Category)
                .ToList();

            var documentItems = itemsFromDb.Select(i => new DocumentItem
            {
                DocumentId = vm.DocumentId,
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
            var selectedIds = documentItems.Select(di => di.ItemId).ToList();
            var itemsFromDb = _context.Item
            .Where(i => selectedIds.Contains(i.ItemId))
            .Include(i => i.Category)
            .ToList();

            foreach (var di in documentItems)
            {
                di.Item = itemsFromDb.First(i => i.ItemId == di.ItemId);
            }

            if (ModelState.IsValid)
            {
                _context.DocumentItem.AddRange(documentItems);
     

                int docId = documentItems.FirstOrDefault().DocumentId;
                var document = await _context.Document.FindAsync(docId);
                if (document == null)
                {
                    return NotFound($"Wystąpił błąd podczas wyliczania wartości dokumentu");
                }

                document.TotalValue = calculateTotalVal(documentItems);
                await _context.SaveChangesAsync();

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
        private decimal calculateTotalVal(List<DocumentItem> items)
        {
            var total = items.Sum(i => i.Item.Price * i.Quantity);
            return total;
        }
    }
}
