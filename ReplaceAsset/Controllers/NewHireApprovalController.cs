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
    public class NewHireApprovalController : Controller
    {
        private readonly ApplicationDbContext _context;

        public NewHireApprovalController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: NewHireApproval
        public async Task<IActionResult> Index()
        {
            // Fetch only new hires that are "waiting for deploy"
            var newHires = await _context.NewHire
                .Where(nh => !nh.StatusCompleted) // StatusCompleted == false means 'waiting for deploy'
                .ToListAsync();

            ViewBag.Id = 1;

            return View(newHires);
        }

        // API ENDPOINT
        [HttpGet]
        public IActionResult GetData()
        {
            var newHire = _context.NewHire
                .Where(nh => !nh.StatusCompleted) // Fetch only "waiting for deploy"
                .Select(a => new
                {
                    id = a.Id,
                    name = a.Name,
                    department = a.Department,
                    designation = a.Designation,
                    serialNumber = a.SerialNumber,
                    device = a.Device,
                    modelAsset = a.ModelAsset,
                    dateOfJoin = a.DateOfJoin.HasValue ? a.DateOfJoin.Value.ToString("dd MMM yyyy HH:mm") : null,
                    statusCompleted = a.StatusCompleted,
                    headsetGiven = a.HeadsetGiven,
                    laptopGiven = a.LaptopGiven,
                    adaptorGiven = a.AdaptorGiven,
                    powerCableGiven = a.PowerCableGiven,
                    bagGiven = a.BagGiven
                })
                .ToList();

            return Json(new { rows = newHire });
        }

        [HttpPost]
        public async Task<IActionResult> UpdateStatus(int id, [FromBody] UpdateStatusViewModel model)
        {
            var newHire = await _context.NewHire.FindAsync(id);
            if (newHire == null)
            {
                return NotFound();
            }

            newHire.StatusCompleted = true;
            newHire.HeadsetGiven = model.HeadsetGiven;
            newHire.LaptopGiven = model.LaptopGiven;
            newHire.AdaptorGiven = model.AdaptorGiven;
            newHire.PowerCableGiven = model.PowerCableGiven;
            newHire.BagGiven = model.BagGiven;

            _context.Update(newHire);
            await _context.SaveChangesAsync();
            TempData["SuccessMessage"] = "Asset New Hire has been deploy successfully!";


            return Ok();
        }

        private bool NewHireExists(int id)
        {
            return _context.NewHire.Any(e => e.Id == id);
        }
    }

    public class UpdateStatusViewModel
    {
        public bool HeadsetGiven { get; set; }
        public bool LaptopGiven { get; set; }
        public bool AdaptorGiven { get; set; }
        public bool PowerCableGiven { get; set; }
        public bool BagGiven { get; set; }
    }
}