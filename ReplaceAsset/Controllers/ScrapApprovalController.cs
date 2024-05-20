using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ReplaceAsset.Data;
using ReplaceAsset.Models;

namespace ReplaceAsset.Controllers
{
    public class ScrapApprovalController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ScrapApprovalController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: ScrapApproval
        public IActionResult Index()
        {
            return View();
        }

        // API ENDPOINT
        [HttpGet]
        public IActionResult GetData()
        {
            var pendingScraps = _context.AssetScrap
                .Where(a => !a.ValidationScrap) // Menampilkan hanya yang pending (ValidationScrap == false)
                .Select(a => new
                {
                    id = a.Id,
                    type = a.Type,
                    serialNumber = a.SerialNumber,
                    location = a.Location,
                    dateInput = a.DateInput.HasValue ? a.DateInput.Value.ToString("dd MMM yyyy") : null,
                    validationScrap = a.ValidationScrap
                })
                .ToList();

            return Json(new { rows = pendingScraps });
        }

        [HttpPost]
        public IActionResult ApproveScrap(int id)
        {
            var assetScrap = _context.AssetScrap.Find(id);
            if (assetScrap == null)
            {
                return NotFound();
            }

            assetScrap.ValidationScrap = true;
            _context.SaveChanges();

            return Ok();
        }
    }
}
/*
        // GET: ScrapApproval/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var assetScrap = await _context.AssetScrap
                .FirstOrDefaultAsync(m => m.Id == id);
            if (assetScrap == null)
            {
                return NotFound();
            }

            return View(assetScrap);
        }

        // GET: ScrapApproval/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: ScrapApproval/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Type,SerialNumber,Location,DateInput,ValidationScrap")] AssetScrap assetScrap)
        {
            if (ModelState.IsValid)
            {
                _context.Add(assetScrap);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(assetScrap);
        }

        // GET: ScrapApproval/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var assetScrap = await _context.AssetScrap.FindAsync(id);
            if (assetScrap == null)
            {
                return NotFound();
            }
            return View(assetScrap);
        }

        // POST: ScrapApproval/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Type,SerialNumber,Location,DateInput,ValidationScrap")] AssetScrap assetScrap)
        {
            if (id != assetScrap.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(assetScrap);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!AssetScrapExists(assetScrap.Id))
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
            return View(assetScrap);
        }

        // GET: ScrapApproval/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var assetScrap = await _context.AssetScrap
                .FirstOrDefaultAsync(m => m.Id == id);
            if (assetScrap == null)
            {
                return NotFound();
            }

            return View(assetScrap);
        }

        // POST: ScrapApproval/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var assetScrap = await _context.AssetScrap.FindAsync(id);
            if (assetScrap != null)
            {
                _context.AssetScrap.Remove(assetScrap);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool AssetScrapExists(int id)
        {
            return _context.AssetScrap.Any(e => e.Id == id);
        }
    }
}
*/