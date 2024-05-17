using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
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

        // GET: NewHires
        public async Task<IActionResult> Index()
        {
            return View(await _context.NewHire.ToListAsync());
        }

        // API ENDPOINT
        [HttpGet]
        public IActionResult GetData()
        {
            var newHire = _context.NewHire
                .Select(a => new
                {
                    id = a.Id,
                    name = a.Name,
                    department = a.Department,
                    designation = a.Designation,
                    serialNumber = a.SerialNumber,
                    device = a.Device,
                    modelAsset = a.ModelAsset,
                    dateOfJoin = a.DateOfJoin.HasValue ? a.DateOfJoin.Value.ToString("dd MMM yyyy") : null,
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
                return RedirectToAction(nameof(Index));
            }
            return View(newHire);
        }
        // GET: NewHires/Edit/5
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
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Department,Designation,Device,SerialNumber,ModelAsset,DateOfJoin,HeadsetGiven,LaptopGiven,AdaptorGiven,PowerCableGiven,BagGiven")] NewHire newHire)
        {
            if (id != newHire.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    // Fetch the existing hire record to get the current status
                    var existingHire = await _context.NewHire.AsNoTracking().FirstOrDefaultAsync(n => n.Id == id);
                    if (existingHire == null)
                    {
                        return NotFound();
                    }

                    // Preserve the existing status if it is completed
                    if (existingHire.StatusCompleted)
                    {
                        newHire.StatusCompleted = true;
                    }

                    _context.Update(newHire);
                    await _context.SaveChangesAsync();
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
                return RedirectToAction(nameof(Index), new { returnFromEdit = true });
            }
            return View(newHire);
        }
        private bool NewHireExists(int id)
        {
            return _context.NewHire.Any(e => e.Id == id);
        }
    }
}
