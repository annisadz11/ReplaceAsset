﻿using System;
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
    public class AssetScrapsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AssetScrapsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: AssetScraps
        public IActionResult Index()
        {
            return View();
        }

        // API ENDPOINT
        [HttpGet]
        public IActionResult GetData()
        {
            var assetScraps = _context.AssetScrap
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

            return Json(new { rows = assetScraps });
        }

        // GET: AssetScraps/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: AssetScraps/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Type,SerialNumber,Location,DateInput,ValidationScrap")] AssetScrap assetScrap)
        {
            if (ModelState.IsValid)
            {
                _context.Add(assetScrap);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Asset Scrap created successfully!";
                return RedirectToAction(nameof(Index));
            }
            return View(assetScrap);
        }

        // GET: AssetScraps/Edit/5
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

        // POST: AssetScraps/Edit/5
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
                    return Json(new { success = true, message = "Asset Scrap updated successfully!" });
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
            }

            return Json(new { success = false, message = "Error updating the Asset Scrap." });
        }

        // GET: AssetScraps/Details/5
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

        // GET: AssetRequests/Delete/5
        public async Task<IActionResult> Delete(int? id, string handler)
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

            if (handler == "Delete")
            {
                _context.AssetScrap.Remove(assetScrap);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Asset Scrap has been deleted successfully.";
                return RedirectToAction(nameof(Index));
            }

            return View(assetScrap);
        }

        // POST: AssetRequests/Delete/5
        [HttpPost]
        public async Task<IActionResult> Delete(int id, string handler)
        {
            var assetScrap = await _context.AssetScrap.FindAsync(id);

            if (assetScrap == null)
            {
                return NotFound();
            }

            if (handler == "Delete")
            {
                _context.AssetScrap.Remove(assetScrap);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Asset Scrap has been deleted successfully.";
                return RedirectToAction(nameof(Index));
            }

            return View(assetScrap);
        }
       
       

        private bool AssetScrapExists(int id)
        {
            return _context.AssetScrap.Any(e => e.Id == id);
        }
    }
}