using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using WmsCore.Data;
using WmsCore.Models;
using WmsCore.ViewModels;

namespace WmsCore.Controllers
{
    public class InventoryController : Controller
    {
        private readonly WmsCoreContext _context;

        public InventoryController(WmsCoreContext context)
        {
            _context = context;
        }

        // GET: Inventories
        public async Task<IActionResult> Index(
            string sortOrder,
            string currentFilter,
            string searchString,
            int? pageNumber,
            int? categoryId,
            DateTime? date)
        {
            ViewData["CurrentSort"] = sortOrder;
            ViewData["NameSortParm"] = String.IsNullOrEmpty(sortOrder) ? "name_desc" : "";
            ViewData["PriceSortParm"] = sortOrder == "Price" ? "price_desc" : "Price";
            ViewData["CurrentFilter"] = searchString;

            DateTime searchDate = date ?? DateTime.Now;

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
            ViewData["Date"] = searchDate;

            IQueryable<Article> items = _context.Article
                                                .Include(i => i.Category)
                                                .Include(i => i.InventoryMovements); //Dodajemy historię magazynową aby obliczyć obecną ilość

            // --- Filtrowanie po wyszukiwanej frazie ---
            if (!String.IsNullOrEmpty(searchString))
            {
                items = items.Where(s =>
                    s.Name.Contains(searchString) || s.Code.Contains(searchString));
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
                    items = items.OrderBy(s => s.NetPrice);
                    break;
                case "price_desc":
                    items = items.OrderByDescending(s => s.NetPrice);
                    break;
                default:
                    items = items.OrderBy(s => s.Name);
                    break;
            }


            //Rzutowanie listy towarów na listę ViewModel oraz obliczenie dla każdego artykułu ilości w magazynie
            var nextDay = searchDate.Date.AddDays(1); //znajdzie wszystkie godziny do północy
            IQueryable<InventoryViewModel> inventories = items.Select(i => new InventoryViewModel()
            {
                Article = i,
                TotalQuantity = i.InventoryMovements.Where(m => m.EffectiveDate < nextDay).Sum(m => m.QuantityChange)
            });

            int pageSize = 10;
            return View(await PaginatedList<InventoryViewModel>.CreateAsync(inventories.AsNoTracking(), pageNumber ?? 1, pageSize));
        }

        // GET: Inventories/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var article = await _context.Article
                .Include(i => i.Category)
                .Include(i => i.InventoryMovements)
                    .ThenInclude(m => m.Document)
                    .ThenInclude(d => d.Contractor)
                .Include(i => i.Attributes)
                    .ThenInclude(ifeat => ifeat.AtrDefinition) // ThenInclude - jeszcze dołączamy definicje cech
                .FirstOrDefaultAsync(m => m.ArticleId == id);

            if (article == null)
            {
                return NotFound();
            }

            var view_model = new InventoryViewModel() { 
            Article = article,
            TotalQuantity = article.InventoryMovements.Sum(m => m.QuantityChange)
            };
            return View(view_model);
        }

    }
}
