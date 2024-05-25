using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ReplaceAsset.Data;
using ReplaceAsset.Models;

namespace ReplaceAsset.Controllers
{
    [Authorize(Roles = "UserAdmin,UserIntern,UserManagerIT")]
    public class UserInternsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public UserInternsController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            return View(await _context.UserInterns.ToListAsync());
        }

        [HttpGet]
        public IActionResult GetData()
        {
            var userInterns = _context.UserInterns
                .Select(u => new
                {
                    id = u.Id,
                    name = u.Name,
                    email = u.Email,
                    userName = u.UserName
                })
                .ToList();

            return Json(new { rows = userInterns });
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,Email,UserName")] UserIntern userIntern)
        {
            if (ModelState.IsValid)
            {
                _context.Add(userIntern);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(userIntern);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.UserInterns == null)
            {
                return NotFound();
            }

            var userIntern = await _context.UserInterns.FindAsync(id);
            if (userIntern == null)
            {
                return NotFound();
            }
            return View(userIntern);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Email,UserName")] UserIntern userIntern)
        {
            if (id != userIntern.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(userIntern);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!UserInternExists(userIntern.Id))
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
            return View(userIntern);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.UserInterns == null)
            {
                return NotFound();
            }

            var userIntern = await _context.UserInterns
                .FirstOrDefaultAsync(m => m.Id == id);
            if (userIntern == null)
            {
                return NotFound();
            }

            return View(userIntern);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.UserInterns == null)
            {
                return Problem("Entity set 'ApplicationDbContext.UserIntern' is null.");
            }
            var userIntern = await _context.UserInterns.FindAsync(id);
            if (userIntern != null)
            {
                _context.UserInterns.Remove(userIntern);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool UserInternExists(int id)
        {
            return _context.UserInterns.Any(e => e.Id == id);
        }
    }
}
