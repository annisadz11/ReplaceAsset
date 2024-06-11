using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ReplaceAsset.Data;
using ReplaceAsset.Models;
using Microsoft.AspNetCore.Authorization;

namespace ReplaceAsset.Controllers
{
    [Authorize(Roles = "UserManagerIT,UserAdmin")]
    public class AssetApprovalController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AssetApprovalController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: AssetRequest
        public IActionResult Index()
        {
            return View();
        }

        ///API ENDPOINT

        [HttpGet]
        public IActionResult GetData(DateTime? startDate, DateTime? endDate)
        {
            var assetRequests = _context.AssetRequest
                .Where(ar =>
                    ar.Status == null && // Filter hanya untuk "waiting for approval"
                    (!startDate.HasValue || ar.RequestDate >= startDate) &&
                    (!endDate.HasValue || ar.RequestDate <= endDate))
                .Select(g => new
                {
                    id = g.Id,
                    name = g.Name,
                    departement = g.Departement,
                    emailUser = g.EmailUser,
                    type = g.Type,
                    serialNumber = g.SerialNumber,
                    baseline = g.Baseline,
                    usageLocation = g.UsageLocation,
                    requestDate = g.RequestDate.HasValue ? g.RequestDate.Value.ToString("dd MMM yyyy  HH:mm") : null,
                    reason = g.Reason,
                    status = g.Status,
                    approvalDate = g.ApprovalDate.HasValue ? g.ApprovalDate.Value.ToString("dd MMM yyyy HH:mm") : null,
                    justify = g.Justify,
                    typeReplace = g.TypeReplace
                }).ToList();

            return Json(assetRequests);
        }

        [HttpPost]
        public async Task<IActionResult> Approve(int id, [FromForm] string justify, [FromForm] string typeReplace)
        {
            var assetRequest = await _context.AssetRequest.FindAsync(id);
            if (assetRequest == null)
            {
                return NotFound();
            }

            assetRequest.Status = true; // Set status to approved
            assetRequest.Justify = justify;
            assetRequest.TypeReplace = typeReplace;
            assetRequest.ApprovalDate = DateTime.Now;

            _context.Update(assetRequest);
            await _context.SaveChangesAsync();

            return Json(new { success = true, message = "Asset request approved successfully!" });
        }

        [HttpPost]
        public async Task<IActionResult> Reject(int id, [FromForm] string justify)
        {
            var assetRequest = await _context.AssetRequest.FindAsync(id);
            if (assetRequest == null)
            {
                return NotFound();
            }

            assetRequest.Status = false; // Set status to rejected
            assetRequest.Justify = justify;
            assetRequest.ApprovalDate = DateTime.Now;

            _context.Update(assetRequest);
            await _context.SaveChangesAsync();

            return Json(new { success = true, message = "Asset request rejected successfully!" });
        }
    }
}
