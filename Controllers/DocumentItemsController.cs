using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Threading.Tasks;
using System.Xml.Linq;
using WmsCore.Controllers;
using WmsCore.Data;
using WmsCore.Definitions;
using WmsCore.Models;
using WmsCore.ViewModels;

namespace Music_Store_Warehouse_App.Controllers
{
    public class DocumentItemsController : BaseController
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
            if (vm.DocumentId == 0)
            {
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


            vm.CategoryList = new SelectList(categories, "CategoryId", "Name", vm.CategoryId);


            IQueryable<Article> articles = _context.Article
                                                .Include(i => i.Category)
                                                .Include(i => i.InventoryMovements);

            // --- Filtrowanie po wyszukiwanej frazie ---
            if (!String.IsNullOrEmpty(vm.SearchString))
            {
                articles = articles.Where(s =>
                    s.Name.Contains(vm.SearchString) || s.Code.Contains(vm.SearchString));
            }

            // --- Filtrowanie po kategorii, jeśli użytkownik wybrał categoryId ---
            if (vm.CategoryId.HasValue)
            {
                articles = articles.Where(i => i.CategoryId == vm.CategoryId.Value);
            }


            switch (vm.SortOrder)
            {
                case "code_desc":
                    articles = articles.OrderByDescending(s => s.Code);
                    break;
                case "Name":
                    articles = articles.OrderBy(s => s.Name);
                    break;
                case "name_desc":
                    articles = articles.OrderByDescending(s => s.Name);
                    break;
                case "Price":
                    articles = articles.OrderBy(s => s.NetPrice);
                    break;
                case "price_desc":
                    articles = articles.OrderByDescending(s => s.NetPrice);
                    break;
                default:
                    articles = articles.OrderBy(s => s.Code);
                    break;
            }

            int pageSize = 15;

            var paginatedList = await PaginatedList<Article>.CreateAsync(articles.AsNoTracking(), vm.PageNumber ?? 1, pageSize);
            vm.Articles = paginatedList;
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
                .Include(d => d.Article)
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
            if (vm.SelectedArticles == null || !vm.SelectedArticles.Any())
            {
                TempData["ErrorMessage"] = "Musisz zaznaczyć przynajmniej jedną pozycję.";
                return RedirectToAction("Index", new
                {
                    DocumentId = vm.DocumentId,
                    DocumentType = vm.DocumentType
                }); // brak wybranych pozycji
            }
            var itemsFromDb = _context.Article
                .Where(i => vm.SelectedArticles.Contains(i.ArticleId))
                .Include(i => i.Category)
                .ToList();

            var documentItems = itemsFromDb.Select(i => new DocumentItem
            {
                DocumentId = vm.DocumentId,
                ArticleId = i.ArticleId,
                Article = i,
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
            var selectedIds = documentItems.Select(di => di.ArticleId).ToList();
            var itemsFromDb = _context.Article
            .Where(i => selectedIds.Contains(i.ArticleId))
            .Include(i => i.Category)
            .ToList();

            //przypisanie obiektów w liście documentsItem na podstawie ich ID ponieważ wysłane żądanie zawiera samo ID bez encji
            foreach (var di in documentItems)
            {
                di.Article = itemsFromDb.First(i => i.ArticleId == di.ArticleId);
            }

            if (!ModelState.IsValid)
            {
                return View(documentItems);
            }

            int docId = documentItems.FirstOrDefault().DocumentId;
            var document = await _context.Document.FindAsync(docId);
            if (document == null)
            {
                return NotFound($"Błąd powiązania dokumentu");
            }

            //Obsługa aktualizacji stanów magazynowych
            try
            {
                await UpdateInventoryAsync(document, documentItems);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                return View(documentItems);

            }


            //Dodanie pozycji dokumentu oraz wyliczenie wartości dokumentu
            _context.DocumentItem.AddRange(documentItems);
            document.TotalNetAmount = CalculateTotalNetValue(documentItems);
            document.TotalGrossAmount = CalculateTotalGrossValue(documentItems);
            await _context.SaveChangesAsync();

            return RedirectToAction("Details", "Documents", new { id = docId });
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
            ViewData["ArticleId"] = new SelectList(_context.Article, "ArticleId", "Code", documentItem.ArticleId);
            return View(documentItem);
        }

        // POST: DocumentItems/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("DocumentItemId,DocumentId,ArticleId,Quantity,UnitOfMeasure")] DocumentItem documentItem)
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
            ViewData["ArticleId"] = new SelectList(_context.Article, "ArticleId", "Code", documentItem.ArticleId);
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
                .Include(d => d.Article)
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
        public async Task<IActionResult> DeleteConfirmed(int id, int docID)
        {
            var documentItem = await _context.DocumentItem.Include(i => i.Document).FirstOrDefaultAsync();
            if (documentItem == null)
            {
                NotifyError("Nie udało się pobrać pozycji.");
                return RedirectToAction("Edit", "Documents", new { id = docID });
            }

            var document = documentItem.Document;
            try
            {
                await CorrectInventory(documentItem);
                //Po pomyślnej korekcji stanów wiemy że ta ilość nie będzie zerowa
                document.TotalNetAmount -= documentItem.Quantity * documentItem.NetPrice;
                document.TotalGrossAmount -= documentItem.Quantity * documentItem.GrossPrice;

                _context.DocumentItem.Remove(documentItem);

                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                NotifyError(ex.Message);
                return RedirectToAction("Edit", "Documents", new { id = docID });
            };
            NotifySuccess("Sukces");
            return RedirectToAction("Edit", "Documents", new { id = docID });
        }
        #region Aktualizacje stanów magazynowych
        private async Task UpdateInventoryAsync(WmsCore.Models.Document document, List<DocumentItem> documentItems)
        {
            var selectedIds = documentItems.Select(i => i.ArticleId);

            //Pobranie istniejących zapisów stanów pozycji
            var movements = await _context.InventoryMovement
                            .Where(i => selectedIds.Contains(i.ArticleId))
                            .ToListAsync();



            if (document.Type.Equals(DocumentTypes.PZ) || document.Type.Equals(DocumentTypes.PW))
            {
                ApplyPZPWInventory(document, documentItems);
            }
            else if (document.Type.Equals(DocumentTypes.WZ) || document.Type.Equals(DocumentTypes.RW))
            {
                try
                {
                    ApplyWZRWInventory(document, documentItems, movements);
                }
                catch (InvalidOperationException ex)
                {
                    throw;
                }

            }
            else
            {
                throw new ArgumentException("Nieprawidłowy typ dokumentu");
            }

        }
        #endregion

        #region Aktualizacje stanów PZ PW
        private void ApplyPZPWInventory(WmsCore.Models.Document document, List<DocumentItem> documentItems)
        {
            foreach (var di in documentItems)
            {
                _context.Add(new InventoryMovement()
                {
                    Article = di.Article,
                    Document = document,
                    QuantityChange = di.Quantity,
                    EffectiveDate = document.OperationDate, //Data zaksięgowania
                });
            }
        }
        #endregion

        #region  Aktualizacje stanów WZ RW
        private void ApplyWZRWInventory(WmsCore.Models.Document document, List<DocumentItem> documentItems, List<InventoryMovement> movements)
        {
            //Wszystkie operacje magazynowe przed do dnia zaksięgowania dokumentu tak aby wiedzieć czy towary na pewno będą na stanie
            var grouped_mvm = movements
                        .Where(x => x.EffectiveDate <= document.OperationDate)
                        .GroupBy(x => x.ArticleId)
                        .Select(group => new
                        {
                            ID = group.Key,
                            AvailableStock = group.Sum(x => x.QuantityChange)
                        });

            foreach (var di in documentItems)
            {
                var stock = grouped_mvm.Where(gr => gr.ID == di.ArticleId).SingleOrDefault();
                if (stock == null)
                {
                    throw new InvalidOperationException($"Brak artykułu [{di.Article.Code}] w magazynie");
                }

                if (stock.AvailableStock < di.Quantity)
                {
                    throw new InvalidOperationException($"Na magazynie nie ma wystarczającej ilości artykułu [{di.Article.Code}], pozostałe zasoby = {stock.AvailableStock} na dzień {document.OperationDate}");
                }

                //Ruch magazynowy wydający towary

                _context.Add(new InventoryMovement()
                {
                    Article = di.Article,
                    Document = document,
                    QuantityChange = -di.Quantity, // MINUS ILOŚĆ - zdejmujemy ze stanu
                    EffectiveDate = document.OperationDate //Wpływ na magazyn zgodnie z datą zaksięgowania dokumentu
                }
                );
            }
        }
        #endregion

        #region Aktualizacje stanów przy usunięciu pozycji dokumentu
        //Wprowadzamy korygujący ruch magazynowy
        //Nie dopuszczamy stanów ujemnych
        private async Task CorrectInventory(DocumentItem documentItem)
        {

            //Pobranie istniejących zapisów stanów danej pozycji
            var stock = await _context.InventoryMovement
                            .Where(i => i.ArticleId == documentItem.ArticleId)
                            .SumAsync(x => x.QuantityChange);

            /*Sprawdzamy zatwierdzone i niezatwierdzone ruchy magazynowe czyli bez warunku   .Where(x => x.EffectiveDate <= DateTime.Now()).
            ponieważ pozwala to uniknąc rozjechania się stanów w przyszłości, pozycje usuwamy teraz a kolejny dokument
            zatwierdza się za kilka dni */
            if (stock - documentItem.Quantity < 0)
            {
                throw new InvalidOperationException("Nie można wykonać operacji, blokada przed stanem ujemnym");
            }

            ApplyCorrection(documentItem);

        }
        #region Korekta stanów magazynowych
        private void ApplyCorrection(DocumentItem documentItem)
        {
            var document = documentItem.Document;
            if (document.Type.Equals(DocumentTypes.WZ) || document.Type.Equals(DocumentTypes.RW))
            {
                _context.Add(new InventoryMovement()
                {
                    Article = documentItem.Article,
                    Document = documentItem.Document,
                    QuantityChange = -documentItem.Quantity, // MINUS ILOŚĆ - zdejmujemy ze stanu
                    EffectiveDate = DateTime.Now //Wpływ na magazyn 
                });
            }
            else
            {
                _context.Add(new InventoryMovement()
                {
                    Article = documentItem.Article,
                    Document = documentItem.Document,
                    QuantityChange = documentItem.Quantity, // PLUS ILOŚĆ
                    EffectiveDate = DateTime.Now //Wpływ na magazyn 
                });
            }
        }
        #endregion

        #endregion
        private bool DocumentItemExists(int id)
        {
            return _context.DocumentItem.Any(e => e.DocumentItemId == id);
        }
        private decimal CalculateTotalNetValue(List<DocumentItem> items)
        {
            var total = items.Sum(i => i.Article.NetPrice * i.Quantity);
            return total;

        }
        private decimal CalculateTotalGrossValue(List<DocumentItem> items)
        {
            var total = items.Sum(i => i.Article.GrossPrice * i.Quantity);
            return total;

        }
    }
}

