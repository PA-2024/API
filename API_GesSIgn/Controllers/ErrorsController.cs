using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using API_GesSIgn.Models;

namespace API_GesSIgn.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ErrorsController : Controller
    {
        private readonly MonDbContext _context;

        public ErrorsController(MonDbContext context)
        {
            _context = context;
        }

        // GET: Errors
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var errors = await _context.Errors.ToListAsync();
            return Json(errors);
        }

        // GET: Errors/Details/5
        [HttpGet("{id}")]
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

            return Json(error);
        }

        // POST: Errors/Create
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] Error error)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _context.Add(error);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(Details), new { id = error.Error_id }, error);
        }

        // PATCH: Errors/{id}/resolve
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

        // DELETE: Errors/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var error = await _context.Errors.FindAsync(id);
            if (error == null)
            {
                return NotFound();
            }

            _context.Errors.Remove(error);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}
