using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using API_GesSIgn.Models;

namespace API_GesSIgn.Controllers
{
    public class ErrorsController : Controller
    {
        private readonly MonDbContext _context;

        public ErrorsController(MonDbContext context)
        {
            _context = context;
        }

        // GET: Errors
        public async Task<IActionResult> Index()
        {
            return View(await _context.Errors.ToListAsync());
        }

        // GET: Errors/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var error = await _context.Errors
                .FirstOrDefaultAsync(m => m.Error_id == id);
            if (error == null)
            {
                return NotFound();
            }

            return View(error);
        }

        // GET: Errors/Create
        public IActionResult Create()
        {
            return View();
        }

        // PATCH: api/Errors/5/resolve
        [HttpPatch("{id}/resolve")]
        public async Task<IActionResult> ResolveError(int id)
        {
            var error = await _context.Errors.FindAsync(id);
            if (error == null)
            {
                return NotFound();
            }

            error.Error_Solved = true;
            _context.Errors.Update(error);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // POST: Errors/Create
        [HttpPost]
        public async Task<IActionResult> Create([Bind("Error_id,Error_Funtion,Error_DateTime,Error_Description,Error_Solved")] Error error)
        {
            if (ModelState.IsValid)
            {
                _context.Add(error);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(error);
        }


        // GET: Errors/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var error = await _context.Errors
                .FirstOrDefaultAsync(m => m.Error_id == id);
            if (error == null)
            {
                return NotFound();
            }

            return View(error);
        }

        // POST: Errors/Delete/5
        [HttpPost, ActionName("Delete")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var error = await _context.Errors.FindAsync(id);
            if (error != null)
            {
                _context.Errors.Remove(error);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ErrorExists(int id)
        {
            return _context.Errors.Any(e => e.Error_id == id);
        }
    }
}
