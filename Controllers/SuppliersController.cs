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
    public class SuppliersController : Controller
    {
        private readonly WmsCoreContext _context;

        public SuppliersController(WmsCoreContext context)
        {
            _context = context;
        }

        // GET: Suppliers
        public async Task<IActionResult> Index()
        {
            var WmsCoreContext = _context.Supplier.Include(s => s.Address);
            return View(await WmsCoreContext.ToListAsync());
        }

        // GET: Suppliers/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var supplier = await _context.Supplier
                .Include(s => s.Address)
                .FirstOrDefaultAsync(m => m.SupplierId == id);
            if (supplier == null)
            {
                return NotFound();
            }

            return View(supplier);
        }

        // GET: Suppliers/Create
        public IActionResult Create()
        { 
            return View();
        }

        // POST: Suppliers/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Supplier supplier)
        {
            if (ModelState.IsValid)
            {
                _context.Add(supplier);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(supplier);
        }

        // GET: Suppliers/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var supplier = await _context.Supplier
                .Include(a => a.Address)    //Dolejamy adres, ponieważ ten edit ma modyfikować i dostawcę i adres
                .FirstOrDefaultAsync(s => s.SupplierId == id); 
            if (supplier == null)
            {
                return NotFound();
            }   
            return View(supplier);
        }

        // POST: Suppliers/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Supplier supplier) //Brak bindowania ponieważ nie działało przesłanie Address i jego pól
        {
            if (id != supplier.SupplierId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    // Pobierz z bazy istniejący Supplier wraz z powiązanym Address
                    var existing = await _context.Supplier
                                                 .Include(s => s.Address)
                                                 .FirstOrDefaultAsync(s => s.SupplierId == id);
                    if (existing == null)
                        return NotFound();

                    // Zaktualizuj pola dostawcy
                    existing.Name = supplier.Name;
                    existing.Email = supplier.Email;

                    // Jeżeli Address bywa null, utwórz nowy:
                    if (existing.Address == null)
                    {
                        existing.Address = new Address();
                        existing.Address.SupplierId = existing.SupplierId;
                    }

                    // Zaktualizuj pola istniejącego Address
                    existing.Address.Street = supplier.Address.Street;
                    existing.Address.City = supplier.Address.City;
                    existing.Address.PostalCode = supplier.Address.PostalCode;

                    // Teraz EF Core zrozumie, że:
                    // - dostawca istnieje → zmienia tylko jego kolumny
                    // - Address istnieje (ma ustawione AddressId i SupplierId) → zmienia tylko kolumny Street/City/PostalCode
                    // Nie będzie próbował wstawić nowego wiersza do tabeli Address.

                    _context.Update(existing);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!SupplierExists(supplier.SupplierId))
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
            return View(supplier);
        }

        // GET: Suppliers/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var supplier = await _context.Supplier
                .Include(s => s.Address)
                .FirstOrDefaultAsync(m => m.SupplierId == id);
            if (supplier == null)
            {
                return NotFound();
            }

            return View(supplier);
        }

        // POST: Suppliers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var supplier = await _context.Supplier.FindAsync(id);
            if (supplier != null)
            {
                _context.Supplier.Remove(supplier);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool SupplierExists(int id)
        {
            return _context.Supplier.Any(e => e.SupplierId == id);
        }
    }
}
