using System;
using System.Collections.Generic;
using System.Diagnostics.Metrics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using WmsCore.Controllers;
using WmsCore.Data;
using WmsCore.Definitions;
using WmsCore.Models;

namespace Music_Store_Warehouse_App.Views
{
    public class ArticlesController : BaseController
    {
        private readonly WmsCoreContext _context;

        public ArticlesController(WmsCoreContext context)
        {
            _context = context;
        }

        // GET: Articles
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


            IQueryable<Article> articles = _context.Article
                                                .Include(i => i.Category);

            // --- Filtrowanie po wyszukiwanej frazie ---
            if (!String.IsNullOrEmpty(searchString))
            {
                articles = articles.Where(s =>
                    s.Name.Contains(searchString) || s.Code.Contains(searchString));
            }

            // --- Filtrowanie po kategorii, jeśli użytkownik wybrał categoryId ---
            if (categoryId.HasValue)
            {
                articles = articles.Where(i => i.CategoryId == categoryId.Value);
            }


            switch (sortOrder)
            {
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
                    articles = articles.OrderBy(s => s.Name);
                    break;
            }

            int pageSize = 10;
            return View(await PaginatedList<Article>.CreateAsync(articles.AsNoTracking(), pageNumber ?? 1, pageSize));
        }

        // GET: articles/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var articles = await _context.Article
                .Include(i => i.Category)
                .Include(i => i.InventoryMovements)
                .Include(i => i.Attributes)
                    .ThenInclude(ifeat => ifeat.AtrDefinition) // ThenInclude - jeszcze dołączamy definicje cech
                .FirstOrDefaultAsync(m => m.ArticleId == id);

            if (articles == null)
            {
                return NotFound();
            }

            return View(articles);
        }


        private void PrepareViewBags()
        {
            ViewBag.CategoryIds = new SelectList(_context.Category, "CategoryId", "CategoryId");
            ViewBag.VatRates = new SelectList(VatRates.GetSymbols());
            ViewBag.Units = new SelectList(Units.GetAllUnits());
        }


        // GET: Articles/Create
        public IActionResult Create()
        {
            PrepareViewBags();
            return View();
        }

        // POST: Articles/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ArticleId,Code,Name,NetPrice,GrossPrice,VatRate,Unit,Description,EAN,CategoryId")] Article article, string action,  // "ShowAttributess" lub "SaveInstrument"
            [FromForm] IList<WmsCore.Models.Attribute> Attributes)
        {
            // Zawsze przygotowujemy ViewBag dropdownów, bo widok ich potrzebuje
            PrepareViewBags();

            // Jeżeli użytkownik wcisnął „Pokaż cechy”
            if (action == "ShowAttributes")
            {
                // Upewnijmy się, że wybrano kategorię
                if (article.CategoryId == 0)
                {
                    ModelState.AddModelError(nameof(article.CategoryId), "Kategoria jest wymagana, aby wyświetlić cechy.");
                }
                else
                {
                    // Jeżeli jest wartość CategoryId, ładujemy od razu cechy dla danej grupy towarów
                    var cat = _context.Category.Find(article.CategoryId);
                    if (cat != null)
                    {
                        var type = cat.Name;
                        ViewBag.AttributesDefinitions = _context.AtrDefinition
                            .Where(a => a.AttributeGroup.Name == type)
                            .ToList();
                    }
                }

                // Zwracamy widok z aktualnym modelem, nawet jeśli np. Name lub Price są puste.

                return View(article);
            }

            // Jeżeli użytkownik wcisnął „Zapisz artykuł”
            else if (action == "Save")
            {
                // Przypisz z POST-a listę cech (może być pusta, jeżeli nie było tabeli)
               // item.Attributes = Attributes;

                if (!ModelState.IsValid)
                {
                    // Aby tabela cech nie zniknęła, musimy znów załadować AttributesDefinitions
                    if (article.CategoryId != 0)
                    {
                        var cat = _context.Category.Find(article.CategoryId);
                        var type = cat.Name;
                        ViewBag.AttributesDefinitions = _context.AtrDefinition
                            .Where(a => a.AttributeGroup.Name == type)
                            .ToList();
                    }
                    return View(article);
                }

                //Obliczenie brutto
                article.GrossPrice = article.NetPrice * (1 + VatRates.GetMultiplier(article.VatRate));
                // Jeśli ModelState jest OK – zapisujemy do bazy:
                _context.Add(article);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            return View(article);
        }


        // GET: Articles/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var item = await _context.Article
                .Include(i => i.Category)
                .Include(i => i.Attributes)
                    .ThenInclude(atr => atr.AtrDefinition)
                .Include(i => i.InventoryMovements)
                .FirstOrDefaultAsync(i => i.ArticleId == id);

            if (item == null)
            {
                return NotFound();
            }

            PrepareViewBags();
            return View(item);
        }

        // POST: Articles/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ArticleId,Code,Name,NetPrice,GrossPrice,VatRate,Unit,Description,EAN,CategoryId")] Article article)
        {
            if (id != article.ArticleId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    //Cena brutto jest zmieniana przez java script lecz w razie błędów kontroler sprawdza wartości
                    //drugi raz:

                    //Zmiana ceny brutto w razie zmiany stawki lub ceny netto
                    var viewGrossPrice = article.NetPrice * (1 + VatRates.GetMultiplier(article.VatRate));
                    if (viewGrossPrice != article.GrossPrice) {
                        article.GrossPrice = viewGrossPrice;
                    }
                    _context.Update(article);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!await ArticleExistsAsync(article.ArticleId))
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
            ViewBag.VatRates = new SelectList(VatRates.GetSymbols());
            ViewBag.Units = new SelectList(Units.GetAllUnits());
            ViewBag.CategoryIds = new SelectList(_context.Category, "CategoryId", "CategoryId", article.CategoryId);
            return View(article);
        }

        // GET: Articles/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var item = await _context.Article
                .Include(i => i.Category)
                .FirstOrDefaultAsync(m => m.ArticleId == id);
            if (item == null)
            {
                return NotFound();
            }

            return View(item);
        }

        // POST: Articles/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var item = await _context.Article.FindAsync(id);
            if (item != null)
            {
                _context.Article.Remove(item);
            }
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (Exception ex) {
                if (ex.InnerException != null && ex.InnerException.ToString().Contains("FK_InventoryMovement")) {
                    NotifyError("Artykuł jest powiązany z dokumentami");
                }
                else {
                    NotifyError(ex.Message + "\n Inner Exception: \n"+ ex.InnerException.ToString());
                }
                return View(item);
            }
            return RedirectToAction(nameof(Index));
        }

        private async Task<bool> ArticleExistsAsync(int id)
        {
            return await _context.Article.AnyAsync(e => e.ArticleId == id);
        }
    }
}
