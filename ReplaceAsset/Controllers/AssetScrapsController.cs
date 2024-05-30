using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
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

        //Endpoint untuk Total Scrap
		[HttpGet]
		public IActionResult GetTotalScraps()
		{
			var totalScraps = _context.AssetScrap.Count();
			return Json(totalScraps);
		}

        [Authorize(Roles = "UserManagerIT,UserAdmin,UserIntern")]
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
                    dateInput = a.DateInput.HasValue ? a.DateInput.Value.ToString("dd MMM yyyy HH:mm") : null,
                    validationScrap = a.ValidationScrap
                })
                .ToList();

            return Json(new { rows = assetScraps });
        }

        [Authorize(Roles = "UserAdmin,UserIntern")]
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
        [Authorize(Roles = "UserAdmin,UserIntern")]
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
                    TempData["SuccessMessage"] = "Asset Scrap updated successfully!";
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
            return Json(new { success = false, message = "Model state is invalid." });
        }

        [Authorize(Roles = "UserManagerIT,UserAdmin,UserIntern")]

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
        [Authorize(Roles = "UserAdmin,UserIntern,UserManagerIT")]

        [HttpPost]
        public async Task<IActionResult> DeleteSelected(List<int> ids)
        {
            try
            {
                var assetScraps = await _context.AssetScrap.Where(r => ids.Contains(r.Id)).ToListAsync();
                if (!assetScraps.Any())
                {
                    return Json(new { success = false, message = "No asset scraps found." });
                }

                _context.AssetScrap.RemoveRange(assetScraps);
                await _context.SaveChangesAsync();

                return Json(new { success = true, message = $"{assetScraps.Count} asset scraps have been deleted successfully." });
            }
            catch (Exception e)
            {
                return Json(new { success = false, message = $"An error occurred: {e.Message}" });
            }
        }
        private bool AssetScrapExists(int id)
        {
            return _context.AssetScrap.Any(e => e.Id == id);
        }
    }
}