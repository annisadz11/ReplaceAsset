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
    [Authorize(Roles = "UserManagerIT,UserAdmin")]
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
            var pendingScraps = _context.AssetScrap
                .Where(a => a.ValidationScrap == false)
                .ToList();

            return View(pendingScraps);
        }

        // API ENDPOINT
        [HttpGet]
        public IActionResult GetData()
        {
            var pendingScraps = _context.AssetScrap
                .Where(a => a.ValidationScrap == false)
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

            // Update validation status
            assetScrap.ValidationScrap = true;
            _context.SaveChanges();
            TempData["SuccessMessage"] = "Approval Scrap created successfully!";
            return Ok();
        }
    }
}
