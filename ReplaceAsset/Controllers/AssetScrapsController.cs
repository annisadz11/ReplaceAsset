using System;
using System.Collections.Generic;
using System.Drawing.Printing;
using System.Linq;
using System.Threading.Tasks;
using ClosedXML.Excel;
using DinkToPdf;
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
        [Authorize(Roles = "UserManagerIT,UserAdmin,UserIntern")]
        [HttpGet]
        public IActionResult GetData(DateTime? startDate, DateTime? endDate)
        {
            var assetScraps = _context.AssetScrap
                .Where(ar =>
                    (!startDate.HasValue || ar.DateInput >= startDate) &&
                    (!endDate.HasValue || ar.DateInput <= endDate))
                .Select(a => new
                {
                    id = a.Id,
                    type = a.Type,
/*                    name = a.Name,
*/                    serialNumber = a.SerialNumber,
                    location = a.Location,
                    dateInput = a.DateInput.HasValue ? a.DateInput.Value.ToString("dd MMM yyyy HH:mm") : null,
                    validationScrap = a.ValidationScrap
                })
                .ToList();

            return Json(new { rows = assetScraps });
        }
        [Authorize(Roles = "UserManagerIT,UserAdmin,UserIntern")]
        [HttpGet]
        public IActionResult GetDataByStatus(string status)
        {
            List<AssetScrap> assetScraps = null;

            switch (status.ToLower())
            {
                case "done":
                    assetScraps = _context.AssetScrap.Where(ar => ar.ValidationScrap == true).ToList();
                    break;
                case "pending":
                    assetScraps = _context.AssetScrap.Where(ar => ar.ValidationScrap == false).ToList();
                    break;
                case "all":
                    assetScraps = _context.AssetScrap.ToList();
                    break;
                default:
                    return BadRequest("Invalid status selected.");
            }

            var result = assetScraps.Select(g => new
            {
                id = g.Id,
                type = g.Type,
                serialNumber = g.SerialNumber,
                location = g.Location,
                dateInput = g.DateInput.HasValue ? g.DateInput.Value.ToString("dd MMM yyyy HH:mm") : null,
                validationScrap = g.ValidationScrap ? "Done" : "Pending"
            }).ToList();

            return Json(result);
        }

        [Authorize(Roles = "UserManagerIT,UserAdmin,UserIntern")]
        public IActionResult ExportToExcel()
        {
            var assetScraps = _context.AssetScrap.ToList();

            using (var workbook = new XLWorkbook())
            {
                // Mengubah nama worksheet menjadi "Asset Scraps"
                var worksheet = workbook.Worksheets.Add("Asset Scraps");

                // Membuat header pada Excel dengan nama kolom
                worksheet.Cell(1, 1).Value = "Type";
                worksheet.Cell(1, 2).Value = "Serial Number";
                worksheet.Cell(1, 3).Value = "Location";
                worksheet.Cell(1, 4).Value = "Date Input";
                worksheet.Cell(1, 5).Value = "Validation Scrap";

                // Mengisi data pada Excel
                for (int i = 0; i < assetScraps.Count; i++)
                {
                    worksheet.Cell(i + 2, 1).Value = assetScraps[i].Type;
                    worksheet.Cell(i + 2, 2).Value = assetScraps[i].SerialNumber;
                    worksheet.Cell(i + 2, 3).Value = assetScraps[i].Location;
                    worksheet.Cell(i + 2, 4).Value = assetScraps[i].DateInput.HasValue ? assetScraps[i].DateInput.Value.ToString("dd MMM yyyy HH:mm") : null;
                    worksheet.Cell(i + 2, 5).Value = assetScraps[i].ValidationScrap ? "Done Scrap" : "Pending";
                }

                using (var stream = new MemoryStream())
                {
                    workbook.SaveAs(stream);
                    var content = stream.ToArray();
                    // Mengubah nama file menjadi "Asset Scraps.xlsx"
                    return File(content, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Asset Scraps.xlsx");
                }
            }
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
        [Authorize(Roles = "UserAdmin,UserManagerIT,UserIntern")]
        [HttpPost]
        public async Task<IActionResult> DeleteSelectedWithStatus([FromBody] DeleteScrapRequest request)
        {
            if (request == null || string.IsNullOrEmpty(request.Status))
            {
                return BadRequest("Invalid request.");
            }

            IEnumerable<AssetScrap> assetScraps = null;

            switch (request.Status.ToLower())
            {
                case "done":
                    assetScraps = await _context.AssetScrap.Where(x => x.ValidationScrap == true).ToListAsync();
                    break;
                case "pending":
                    assetScraps = await _context.AssetScrap.Where(x => x.ValidationScrap == false).ToListAsync();
                    break;
                case "all":
                    assetScraps = await _context.AssetScrap.ToListAsync();
                    break;
                default:
                    return BadRequest("Invalid status selected.");
            }

            if (!assetScraps.Any())
            {
                return NotFound("No asset scraps found with the specified status.");
            }

            _context.AssetScrap.RemoveRange(assetScraps);
            await _context.SaveChangesAsync();

            return Json(new { success = true, message = "Asset scraps with the selected status were deleted successfully." });
        }

        public class DeleteScrapRequest
        {
            public string Status { get; set; }
            public List<int> Ids { get; set; }
        }

        [Authorize(Roles = "UserManagerIT,UserAdmin,UserIntern")]
        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var assetScrap = await _context.AssetScrap.FindAsync(id);
            if (assetScrap == null)
            {
                return Json(new { success = false, message = "Asset scrap not found." });
            }

            _context.AssetScrap.Remove(assetScrap);
            await _context.SaveChangesAsync();
            return Json(new { success = true, message = "Asset scrap deleted successfully." });
        }


        private bool AssetScrapExists(int id)
        {
            return _context.AssetScrap.Any(e => e.Id == id);
        }

    }
}