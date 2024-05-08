using API_GesSIgn.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API_GesSIgn.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class SchoolController : ControllerBase
    {
        private readonly MonDbContext _context;

        public SchoolController(MonDbContext context)
        {
            _context = context;
        }

        // GET: School
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var schools = await _context.Schools.ToListAsync();
            return Ok(schools); // Returns JSON list of schools
        }

        // GET: School/Details/5
        [HttpGet("{id}")]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var school = await _context.Schools
                .FirstOrDefaultAsync(m => m.School_Id == id);
            if (school == null)
            {
                return NotFound();
            }

            return Ok(school); // Returns JSON object of a single school
        }

        // POST: School/Create
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] School school)
        {
            if (ModelState.IsValid)
            {
                _context.Add(school);
                await _context.SaveChangesAsync();
                return CreatedAtAction(nameof(Details), new { id = school.School_Id }, school); // Return a 201 status code
            }
            return BadRequest(ModelState); // Return 400 status code with ModelState
        }

        // POST: School/Edit/5
        [HttpPost("{id}")]
        public async Task<IActionResult> Edit(int id, [FromBody] School school)
        {
            if (id != school.School_Id)
            {
                return BadRequest();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(school);
                    await _context.SaveChangesAsync();
                    return Ok(school);
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!SchoolExists(school.School_Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
            }
            return BadRequest(ModelState);
        }

        // DELETE: School/Delete/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var school = await _context.Schools
                .FirstOrDefaultAsync(m => m.School_Id == id);
            if (school == null)
            {
                return NotFound();
            }

            _context.Schools.Remove(school);
            await _context.SaveChangesAsync();
            return NoContent(); // Successful response with no content
        }

        private bool SchoolExists(int id)
        {
            return _context.Schools.Any(e => e.School_Id == id);
        }
    }
}
