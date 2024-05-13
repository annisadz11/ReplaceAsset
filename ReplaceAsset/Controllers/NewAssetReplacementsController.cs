using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
                                       n.AssetRequest.ApprovalDate.Value.ToString("dd MMMM yyyy", new System.Globalization.CultureInfo("id-ID")) : null,
                    newType = n.NewType ?? "",
                    newSerialNumber = n.NewSerialNumber ?? "",
                    dateReplace = n.DateReplace.HasValue ?
                          n.DateReplace.Value.ToString("dd MMMM yyyy", new System.Globalization.CultureInfo("id-ID")) : null,
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
                    dateReplace = n.DateReplace.HasValue ? n.DateReplace.Value.ToString("dd MMMM yyyy", new System.Globalization.CultureInfo("id-ID")) : null
                })
        .FirstOrDefaultAsync();

            if (newAssetReplacement == null)
            {
                return NotFound();
            }

            return Ok(newAssetReplacement);
        }

        // POST: NewAssetReplacements/Create
        // Create method to update NewAssetReplacement details
        [HttpPost]
        public async Task<IActionResult> Update(int id, string newType, string newSerialNumber, DateTime? dateReplace)
        {
            var newAssetReplacement = await _context.NewAssetReplacement.FindAsync(id);

            if (newAssetReplacement == null)
            {
                return NotFound();
            }

            newAssetReplacement.NewType = newType;
            newAssetReplacement.NewSerialNumber = newSerialNumber;
            newAssetReplacement.DateReplace = dateReplace;

            try
            {
                _context.Update(newAssetReplacement);
                await _context.SaveChangesAsync();
                return Json(new { success = true, message = "Update successful!" });
            }
            catch (DbUpdateException ex)
            {
                // Log the exception details
                Console.WriteLine(ex.ToString());
                return Json(new { success = false, message = "Error updating the request." });
            }
        }

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
    }
}