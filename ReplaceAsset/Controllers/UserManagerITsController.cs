using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ReplaceAsset.Data;
using ReplaceAsset.Models;

namespace ReplaceAsset.Controllers
{
    [Authorize(Roles = "UserAdmin,UserIntern,UserManagerIT")]
    public class UserManagerITsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public UserManagerITsController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            return View(await _context.UserManagerITs.ToListAsync());
        }

        [HttpGet]
        public IActionResult GetData()
        {
            var userManagerITs = _context.UserManagerITs
                .Select(u => new
                {
                    id = u.Id,
                    name = u.Name,
                    email = u.Email,
                    userName = u.UserName
                })
                .ToList();

            return Json(new { rows = userManagerITs });
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,Email,UserName")] UserManagerIT userManagerIT)
        {
            if (ModelState.IsValid)
            {
                _context.Add(userManagerIT);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(userManagerIT);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.UserManagerITs == null)
            {
                return NotFound();
            }

            var userManagerIT = await _context.UserManagerITs.FindAsync(id);
            if (userManagerIT == null)
            {
                return NotFound();
            }
            return View(userManagerIT);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Email,UserName")] UserManagerIT userManagerIT)
        {
            if (id != userManagerIT.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(userManagerIT);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!UserManagerITExists(userManagerIT.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(userManagerIT);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.UserManagerITs == null)
            {
                return NotFound();
            }

            var userManagerIT = await _context.UserManagerITs
                .FirstOrDefaultAsync(m => m.Id == id);
            if (userManagerIT == null)
            {
                return NotFound();
            }

            return View(userManagerIT);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.UserManagerITs == null)
            {
                return Problem("Entity set 'ApplicationDbContext.UserManagerIT' is null.");
            }
            var userManagerIT = await _context.UserManagerITs.FindAsync(id);
            if (userManagerIT != null)
            {
                _context.UserManagerITs.Remove(userManagerIT);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool UserManagerITExists(int id)
        {
            return _context.UserManagerITs.Any(e => e.Id == id);
        }
    }
}
