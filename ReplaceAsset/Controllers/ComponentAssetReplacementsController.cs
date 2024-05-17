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
    public class ComponentAssetReplacementsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ComponentAssetReplacementsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: ComponentAssetReplacements
        public IActionResult Index()
        {
            var componentAssetReplacements = _context.ComponentAssetReplacement.Include(n => n.AssetRequest).ToList();
            return View(componentAssetReplacements);
        }

        // API ENDPOINT
        [HttpGet]
        public IActionResult GetData()
        {
            var componentAssetReplacements = _context.ComponentAssetReplacement
                .Select(n => new
                {
                    id = n.Id,
                    assetRequestName = n.AssetRequest.Name,
                    assetRequestDepartment = n.AssetRequest.Departement,
                    assetRequestType = n.AssetRequest.Type,
                    assetRequestSerialNumber = n.AssetRequest.SerialNumber,
                    assetRequestBaseline = n.AssetRequest.Baseline,
                    assetRequestUsageLocation = n.AssetRequest.UsageLocation,
                    assetRequestReason = n.AssetRequest.Reason,
                    assetRequestJustify = n.AssetRequest.Justify,
                    assetRequestTypeReplace = n.AssetRequest.TypeReplace,
                    assetRequestApprovalDate = n.AssetRequest.ApprovalDate.HasValue ? n.AssetRequest.ApprovalDate.Value.ToString("dd MMM yyyy") : null,
                    name = n.Name,
                    validationReplace = n.ValidationReplace,
                    componentReplaceDate = n.ComponentReplaceDate.HasValue ? n.ComponentReplaceDate.Value.ToString("dd MMM yyyy") : null
                })
                .ToList();

            return Json(new { rows = componentAssetReplacements });
        }

        //UPDATEMODAL
        [HttpPost]
        public async Task<IActionResult> UpdateComponentReplacement(int id, DateTime? componentReplaceDate)
        {
            var componentAssetReplacement = await _context.ComponentAssetReplacement.FindAsync(id);

            if (componentAssetReplacement == null)
            {
                return Json(new { success = false, message = "Data not found." });
            }

            // Jika ComponentReplaceDate dan ValidationReplace belum memiliki nilai, baru diupdate
            if (!componentAssetReplacement.ComponentReplaceDate.HasValue && !componentAssetReplacement.ValidationReplace)
            {
                componentAssetReplacement.ComponentReplaceDate = componentReplaceDate;
                componentAssetReplacement.ValidationReplace = componentReplaceDate.HasValue;
            }

            try
            {
                await _context.SaveChangesAsync();
                return Json(new { success = true, message = "Component replacement updated successfully." });
            }
            catch (DbUpdateException ex)
            {
                // Log detail exception
                Console.WriteLine(ex.ToString());
                return Json(new { success = false, message = "Error updating component replacement: " + ex.Message });
            }
        }

        // GET: ComponentAssetReplacements/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var componentAssetReplacement = await _context.ComponentAssetReplacement
                .Include(n => n.AssetRequest)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (componentAssetReplacement == null)
            {
                return NotFound();
            }

            return View(componentAssetReplacement);
        }

    }
}