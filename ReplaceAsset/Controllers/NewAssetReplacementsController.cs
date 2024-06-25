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
namespace ReplacementAsset.Controllers
{
    public class NewAssetReplacementsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public NewAssetReplacementsController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult GetToTalNewAssetReplacements()
        {
            var totalNewAssetReplacements = _context.NewAssetReplacement.Count();
            return Json(new { totalNewAssetReplacements });

        }

        [Authorize(Roles = "UserManagerIT,UserAdmin,UserIntern")]

        // GET: NewAssetReplacement
        public IActionResult Index()
        {
            return View();
        }

        // API ENDPOINT
        [HttpGet]
        public IActionResult GetData()
        {
            var newAssetReplacementsData = _context.NewAssetReplacement
                .Include(n => n.AssetRequest) // Pastikan Anda telah Include AssetRequest
                .Where(n => n.AssetRequest.Status == true && n.AssetRequest.TypeReplace == "NewAssetReplacement")
                .Select(n => new
                {
                    id = n.Id,
                    assetRequestName = n.AssetRequest.Name, // Pastikan properti ini match di view
                    assetRequestDepartment = n.AssetRequest.Departement,
                    assetRequestType = n.AssetRequest.Type,
                    assetRequestSerialNumber = n.AssetRequest.SerialNumber,
                    assetRequestBaseline = n.AssetRequest.Baseline,
                    assetRequestUsageLocation = n.AssetRequest.UsageLocation,
                    assetRequestReason = n.AssetRequest.Reason,
                    assetRequestJustify = n.AssetRequest.Justify,
                    assetRequestTypeReplace = n.AssetRequest.TypeReplace,
                    assetRequestApprovalDate = n.AssetRequest.ApprovalDate.HasValue ?
                                       n.AssetRequest.ApprovalDate.Value.ToString("dd MMMM yyyy HH:mm", new System.Globalization.CultureInfo("id-ID")) : null,
                    newType = n.NewType ?? "",
                    newSerialNumber = n.NewSerialNumber ?? "",
                    dateReplace = n.DateReplace.HasValue ?
                          n.DateReplace.Value.ToString("dd MMMM yyyy HH:mm", new System.Globalization.CultureInfo("id-ID")) : null,
                })
        .ToList();

            return Json(new { rows = newAssetReplacementsData });
        }

        //GETBYIDUPDATEMODAL
        [HttpGet]
        public async Task<IActionResult> GetById(int id)
        {
            var newAssetReplacement = await _context.NewAssetReplacement
                .Where(n => n.Id == id)
                .Select(n => new
                {
                    id = n.Id,
                    newType = n.NewType,
                    newSerialNumber = n.NewSerialNumber,
                    dateReplace = n.DateReplace.HasValue ? n.DateReplace.Value.ToString("dd MMMM yyyy HH:mm", new System.Globalization.CultureInfo("id-ID")) : null
                })
        .FirstOrDefaultAsync();

            if (newAssetReplacement == null)
            {
                return NotFound();
            }

            return Ok(newAssetReplacement);
        }
        [Authorize(Roles = "UserAdmin,UserIntern")]
        [HttpPost]
        public async Task<IActionResult> Update(int id, string newType, string newSerialNumber, DateTime? dateReplace)
        {
            var newAssetReplacement = await _context.NewAssetReplacement.FindAsync(id);

            if (newAssetReplacement == null)
            {
                return Json(new { success = false, message = "Data not found." });
            }

            // Jika DateReplace belum memiliki nilai, baru diupdate
            if (!newAssetReplacement.DateReplace.HasValue)
            {
                newAssetReplacement.NewType = newType;
                newAssetReplacement.NewSerialNumber = newSerialNumber;
                newAssetReplacement.DateReplace = dateReplace;
            }

            try
            {
                await _context.SaveChangesAsync();
                return Json(new { success = true, message = "New Asset Replacement updated successfully." });
            }
            catch (DbUpdateException ex)
            {
                // Log detail exception
                Console.WriteLine(ex.ToString());
                return Json(new { success = false, message = "Error updating New Asset Replacement: " + ex.Message });
            }
        }

        [Authorize(Roles = "UserManagerIT,UserAdmin,UserIntern")]

        // GET: NewAssetReplacements/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var newAssetReplacement = await _context.NewAssetReplacement
                .Include(n => n.AssetRequest)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (newAssetReplacement == null)
            {
                return NotFound();
            }

            return View(newAssetReplacement);
        }

        [Authorize(Roles = "UserAdmin")]
        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var newAssetReplacement = await _context.NewAssetReplacement.FindAsync(id);
            if (newAssetReplacement == null)
            {
                return Json(new { success = false, message = "New Asset Replacement not found." });
            }
            _context.NewAssetReplacement.Remove(newAssetReplacement);
            await _context.SaveChangesAsync();
            return Json(new { success = true, message = "Data New Asset Replacement deleted successfully." });
        }
    }
}