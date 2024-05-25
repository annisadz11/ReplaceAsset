using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ReplaceAsset.Data;
using ReplaceAsset.Models;

namespace ReplaceAsset.Controllers
{
    [Authorize(Roles = "UserAdmin,UserIntern,UserManagerIT")]
    public class UserAdminsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public UserAdminsController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            return View(await _context.UserAdmins.ToListAsync());
        }

        [HttpGet]
        public IActionResult GetData()
        {
            var userAdmins = _context.UserAdmins
                .Select(u => new
                {
                    id = u.Id,
                    name = u.Name,
                    email = u.Email,
                    userName = u.UserName
                })
                .ToList();

            return Json(new { rows = userAdmins });
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,Email,UserName")] UserAdmin userAdmin)
        {
            if (ModelState.IsValid)
            {
                _context.Add(userAdmin);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(userAdmin);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.UserAdmins == null)
            {
                return NotFound();
            }

            var userAdmin = await _context.UserAdmins.FindAsync(id);
            if (userAdmin == null)
            {
                return NotFound();
            }
            return View(userAdmin);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Email,UserName")] UserAdmin userAdmin)
        {
            if (id != userAdmin.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(userAdmin);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!UserAdminExists(userAdmin.Id))
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
            return View(userAdmin);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.UserAdmins == null)
            {
                return NotFound();
            }

            var userAdmin = await _context.UserAdmins
                .FirstOrDefaultAsync(m => m.Id == id);
            if (userAdmin == null)
            {
                return NotFound();
            }

            return View(userAdmin);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.UserAdmins == null)
            {
                return Problem("Entity set 'ApplicationDbContext.UserAdmins' is null.");
            }
            var userAdmin = await _context.UserAdmins.FindAsync(id);
            if (userAdmin != null)
            {
                _context.UserAdmins.Remove(userAdmin);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool UserAdminExists(int id)
        {
            return _context.UserAdmins.Any(e => e.Id == id);
        }
    }
}