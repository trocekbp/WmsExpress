using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using WmsCore.Data;
using WmsCore.Definitions;
using WmsCore.Models;

namespace Music_Store_Warehouse_App.Controllers
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
            var wmsCoreContext = _context.Document.Include(d => d.Contractor);
            return View(await wmsCoreContext.ToListAsync());
        }

        // GET: Documents/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var document = await _context.Document
                .Include(d => d.Contractor)
                .Include(d => d.DocumentItems)
                    .ThenInclude(di => di.Article)
                        .ThenInclude(i => i.Category)
                .FirstOrDefaultAsync(m => m.DocumentId == id);
            if (document == null)
            {
                return NotFound();
            }

            return View(document);
        }

        // GET: Documents/Create
        public IActionResult Create(DocumentTypes? type)
        {
            if (type == null)
            {
                throw new InvalidOperationException("Nie przekazano typu dokumentu");

            }

            //Podział ze względu na dostawców i odbiorców
            if (type == DocumentTypes.PZ || type == DocumentTypes.PW)
            {
                ViewData["ContractorId"] = new SelectList(_context.Contractor.Where(i => i.IsContractor), "ContractorId", "Name");
            }
            else if (type == DocumentTypes.WZ || type == DocumentTypes.RW)
            {
                ViewData["ContractorId"] = new SelectList(_context.Contractor.Where(i => i.IsCustomer), "ContractorId", "Name");
            }
            else {
                ViewData["ContractorId"] = new SelectList(_context.Contractor, "ContractorId", "Name");
            }

               
            ViewBag.Type = type;
            var document = new Document(); //inicjalizacja i przekazanie daty wystawienia
            return View(document);
        }

        // POST: Documents/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Document document)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Type = document.Type;
                ViewData["ContractorId"] = new SelectList(_context.Contractor, "ContractorId", "Name", document.ContractorId);
                return View(document);
            }
            try
            {
                document.Number = await GenerateDocumentNumber(document.Date, document.Type); //generowanie unikalnego numeru dokumentu na podstawie daty wystawienia
                document.CreationDate = DateTime.Now;

                _context.Add(document);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index", "DocumentItems", new { documentId = document.DocumentId, documentType = document.Type });
            }
            catch(InvalidOperationException ex) {
                ModelState.AddModelError(string.Empty, ex.Message);
                return View(document);
            }
            
           
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
            ViewData["ContractorId"] = new SelectList(_context.Contractor, "ContractorId", "Name", document.ContractorId);
            return View(document);
        }

        // POST: Documents/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("DocumentId,Type,Date,OperationDate,TotalValue,Description,ContractorId")] Document document)
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
                    if (!await _context.Document.AnyAsync(d => d.DocumentId == document.DocumentId))
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
            ViewData["ContractorId"] = new SelectList(_context.Contractor, "ContractorId", "Name", document.ContractorId);
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
                .Include(d => d.Contractor)
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

        private async Task<string> GenerateDocumentNumber(DateTime docDate, DocumentTypes docType) {

            var sql = "SELECT dbo.fn_GenerateDocumentNumber(@dateParam, @typeParam)";
            string docNumber = string.Empty;
            using (var connection = new SqlConnection(_context.Database.GetConnectionString()))
            {
                docNumber = await connection.ExecuteScalarAsync<string>(sql, new { dateParam = docDate, typeParam = docType.ToString()});
            }

            if (docNumber == null || string.IsNullOrEmpty(docNumber))
                throw new InvalidOperationException("Nie udało się wygenerować numeru dokumentu.");

            if (docNumber == "LIMIT")
                throw new InvalidOperationException(
                    "Przekroczono limit 9999 dokumentów w roku. Skontaktuj się z producentem systemu."
                );
            if (docNumber == "INVALID")
                throw new InvalidOperationException(
                    "Niepoprawny typ dokumentu."
                );

            return docNumber;
        }


    }
}
