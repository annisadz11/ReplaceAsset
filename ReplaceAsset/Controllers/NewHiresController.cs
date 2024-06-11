using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ClosedXML.Excel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using NuGet.ContentModel;
using ReplaceAsset.Data;
using ReplaceAsset.Models;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace ReplaceAsset.Controllers
{
    public class NewHiresController : Controller
    {
        private readonly ApplicationDbContext _context;

        public NewHiresController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult GetTotalNewHires()
        {
            var totalNewHires = _context.NewHire.Count();
            return Json(totalNewHires);
        }

        // Endpoint untuk Ekspor ke Excel
        public IActionResult ExportToExcel()
        {
            var newHires = _context.NewHire.ToList();

            using (var workbook = new XLWorkbook())
            {
                // Mengubah nama worksheet menjadi "New Hires" dengan spasi
                var worksheet = workbook.Worksheets.Add("New Hires");

                // Membuat header pada Excel dengan nama kolom memiliki spasi yang sesuai
                worksheet.Cell(1, 1).Value = "Name";
                worksheet.Cell(1, 2).Value = "Department";
                worksheet.Cell(1, 3).Value = "Designation";
                worksheet.Cell(1, 4).Value = "Serial Number";
                worksheet.Cell(1, 5).Value = "Device";
                worksheet.Cell(1, 6).Value = "Model Asset";
                worksheet.Cell(1, 7).Value = "Date Of Join";
                worksheet.Cell(1, 8).Value = "Status Completed";

                // Mengisi data pada Excel
                for (int i = 0; i < newHires.Count; i++)
                {
                    worksheet.Cell(i + 2, 1).Value = newHires[i].Name;
                    worksheet.Cell(i + 2, 2).Value = newHires[i].Department;
                    worksheet.Cell(i + 2, 3).Value = newHires[i].Designation;
                    worksheet.Cell(i + 2, 4).Value = newHires[i].SerialNumber;
                    worksheet.Cell(i + 2, 5).Value = newHires[i].Device;
                    worksheet.Cell(i + 2, 6).Value = newHires[i].ModelAsset;
                    worksheet.Cell(i + 2, 7).Value = newHires[i].DateOfJoin.HasValue ? newHires[i].DateOfJoin.Value.ToString("dd MMM yyyy HH:mm") : null;
                    worksheet.Cell(i + 2, 8).Value = newHires[i].StatusCompleted ? "Done Deploy" : "Waiting for Deploy";
                }

                using (var stream = new MemoryStream())
                {
                    workbook.SaveAs(stream);
                    var content = stream.ToArray();
                    // Mengubah nama file menjadi "New Hires.xlsx" dengan spasi
                    return File(content, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "New Hires.xlsx");
                }
            }
        }

        [Authorize(Roles = "UserManagerIT,UserAdmin,UserIntern")]

        [Authorize(Roles = "UserManagerIT,UserAdmin,UserIntern")]

        // GET: NewHires
        public async Task<IActionResult> Index()
        {
            return View(await _context.NewHire.ToListAsync());
        }

        // API ENDPOINT
        [Authorize(Roles = "UserManagerIT,UserAdmin,UserIntern")]
        [HttpGet]
        public IActionResult GetData(DateTime? startDate, DateTime? endDate)
        {
            var newHireQuery = _context.NewHire.AsQueryable();

            if (startDate.HasValue && endDate.HasValue)
            {
                newHireQuery = newHireQuery.Where(a => a.DateOfJoin >= startDate && a.DateOfJoin <= endDate);
            }

            var newHire = newHireQuery
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
        [Authorize(Roles = "UserManagerIT,UserAdmin,UserIntern")]
        [HttpGet]
        public IActionResult GetDataByStatus(string status)
        {
            List<NewHire> newHires = null;

            switch (status.ToLower())
            {
                case "done":
                    newHires = _context.NewHire.Where(nh => nh.StatusCompleted == true).ToList();
                    break;
                case "waiting":
                    newHires = _context.NewHire.Where(nh => nh.StatusCompleted == false).ToList();
                    break;
                default:
                    return BadRequest("Invalid status selected.");
            }

            var result = newHires.Select(nh => new
            {
                id = nh.Id,
                name = nh.Name,
                department = nh.Department,
                designation = nh.Designation,
                serialNumber = nh.SerialNumber,
                device = nh.Device,
                modelAsset = nh.ModelAsset,
                dateOfJoin = nh.DateOfJoin.HasValue ? nh.DateOfJoin.Value.ToString("dd MMM yyyy HH:mm") : null,
                statusCompleted = nh.StatusCompleted
            }).ToList();

            return Json(result);
        }

        [Authorize(Roles = "UserManagerIT,UserAdmin,UserIntern")]

        // GET: NewHires/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var newHire = await _context.NewHire.FindAsync(id);
            if (newHire == null)
            {
                return NotFound();
            }
            return View(newHire);
        }

        [Authorize(Roles = "UserIntern,UserAdmin")]

        // GET: NewHires/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: NewHires/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(NewHire newHire)
        {
            if (ModelState.IsValid)
            {
                _context.Add(newHire);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "New Hire successfully input!";
                return RedirectToAction(nameof(Index));
            }
            return View(newHire);
        }





        [Authorize(Roles = "UserIntern,UserAdmin")]
        // GET: NewHires/Edit/5
        [HttpGet]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var newHire = await _context.NewHire.FindAsync(id);
            if (newHire == null)
            {
                return NotFound();
            }
            return View(newHire);
        }


        // POST: NewHires/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Department,Designation,Device,SerialNumber,ModelAsset,DateOfJoin,StatusCompleted,HeadsetGiven,LaptopGiven,AdaptorGiven,PowerCableGiven,BagGiven")] NewHire newHire)
        {
            if (id != newHire.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(newHire);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index), new { returnFromEdit = true });
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!NewHireExists(newHire.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
            }
            return View(newHire);
        }
        [Authorize(Roles = "UserAdmin,UserManagerIT,UserIntern")]
        public async Task<IActionResult> DeleteSelectedWithStatus([FromBody] DeleteNewHire request)
        {
            if (request == null || !request.Ids.Any())
            {
                return BadRequest("No items selected for deletion.");
            }

            IEnumerable<NewHire> newHires;

            switch (request.Status.ToLower())
            {
                case "done": // Memetakan "done" ke status yang sesuai
                    newHires = await _context.NewHire.Where(ar => request.Ids.Contains(ar.Id) && ar.StatusCompleted == true).ToListAsync();
                    break;
                case "waiting": // sudah sesuai
                    newHires = await _context.NewHire.Where(ar => request.Ids.Contains(ar.Id) && ar.StatusCompleted == false).ToListAsync();
                    break;
                default:
                    return BadRequest("Invalid status selected.");
            }

            if (!newHires.Any())
            {
                return NotFound("No requests found with the specified status.");
            }

            _context.NewHire.RemoveRange(newHires);
            await _context.SaveChangesAsync();

            return Json(new { success = true, message = "Selected data new hires were deleted successfully." });
        }

        private bool NewHireExists(int id)
        {
            return _context.NewHire.Any(e => e.Id == id);
        }

        // Additional Model for Delete New Hire
        public class DeleteNewHire
        {
            public List<int> Ids { get; set; }
            public string Status { get; set; }
        }
    }
}