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
    public class DocumentsController : Controller
    {
        private readonly WmsCoreContext _context;

        public DocumentsController(WmsCoreContext context)
        {
            _context = context;
        }

        // GET: Documents
        public async Task<IActionResult> Index()
        {
            return View(await _context.Document
                .Include(i => i.DocumentInstruments)
                    .ThenInclude(instr => instr.Instrument)
                .OrderByDescending(doc => doc.Date)
                .ToListAsync());
        }

        // GET: Documents/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var document = await _context.Document
                .Include(i => i.DocumentInstruments)
                    .ThenInclude(d => d.Instrument)
                        .ThenInclude(c => c.Category)
                 .Include(i => i.DocumentInstruments)
                    .ThenInclude(d => d.Instrument)
                        .ThenInclude(s => s.Supplier)
                .FirstOrDefaultAsync(m => m.DocumentId == id);

            

            if (document == null)
            {
                return NotFound();
            }

            var supplierId = document.DocumentInstruments
                               .Select(di => di.Instrument?.Supplier?.SupplierId)
                               .FirstOrDefault();

            var supplier = await _context.Supplier
                                         .Include(s => s.Address)
                                         .FirstOrDefaultAsync(s => s.SupplierId == supplierId);
            

                        ViewBag.Supplier = supplier;

            return View(document);
        }

        // GET: Documents/Create?supplierId=5&selectedInstrumentIds=2&selectedInstrumentIds=7
        public IActionResult Create(int? supplierId, List<int> selectedInstrumentIds)
        {
            var supplier = _context.Supplier
                            .Include(i => i.Address)
                           .FirstOrDefault(i => i.SupplierId == supplierId);

            if (supplier == null)
                return NotFound();

            var instrument_list = _context.Instrument
                        .Where(i => selectedInstrumentIds.Contains(i.InstrumentId))
                        .Include(i => i.Category)
                        .Include(i => i.Supplier)   
                        .ToList();

            var documentItems = new List<DocumentInstrument>();
            foreach(var item in instrument_list) {
                documentItems.Add(new DocumentInstrument
                {
                    Instrument = item,
                    InstrumentId = item.InstrumentId
                });
            }
            var document = new Document()
            {
                Type = Models.Enums.DocumentType.PZ,
                DocumentInstruments = documentItems
                
            };

            //Opcjonalnie przekazujemy do ViewBag dodatkowe dane (np. nazwa dostawcy)
            ViewBag.Supplier = supplier;

            return View(document);
        }

        // POST: Documents/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("DocumentId, Type")] Document document,
            [FromForm] IList<DocumentInstrument> documentInstruments)
        {
            document.DocumentInstruments = documentInstruments;
            if (ModelState.IsValid)
            {
                document.Date = DateTime.Now;

                //Aktualizacja stanu
                foreach (var item in document.DocumentInstruments) {
                    var instrumentInventory = _context.InstrumentInventory
                                    .FirstOrDefault(i => i.InstrumentId == item.InstrumentId);
                    if (instrumentInventory == null)
                    {
                        _context.InstrumentInventory.Add(new InstrumentInventory() //Jeśli jeszcze nie ma takiego instrumentu w magazynie
                        {
                            InstrumentId = item.InstrumentId,
                            Quantity = item.Quantity
                        });
                    }
                    else {
                        instrumentInventory.Quantity += item.Quantity;
                    }
                
                }
                _context.Add(document);
                //Aktualizacja stanu magazynowego
                // var inventory = _context.
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            return View(document);
        }

        // GET: Documents/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var document = await _context.Document.FindAsync(id);
            if (document == null)
            {
                return NotFound();
            }
            return View(document);
        }

        // POST: Documents/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("DocumentId,Type,Date")] Document document)
        {
            if (id != document.DocumentId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(document);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!DocumentExists(document.DocumentId))
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
            return View(document);
        }

        // GET: Documents/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var document = await _context.Document
                .FirstOrDefaultAsync(m => m.DocumentId == id);
            if (document == null)
            {
                return NotFound();
            }

            return View(document);
        }

        // POST: Documents/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var document = await _context.Document.FindAsync(id);
            if (document != null)
            {
                _context.Document.Remove(document);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool DocumentExists(int id)
        {
            return _context.Document.Any(e => e.DocumentId == id);
        }
    }
}
