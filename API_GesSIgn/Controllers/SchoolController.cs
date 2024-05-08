using API_GesSIgn.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API_GesSIgn.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class SchoolController : Controller
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
            return View(await _context.Schools.ToListAsync());
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

            return View(school);
        }

        // POST: School/Create
        [HttpPost]
        public async Task<IActionResult> Create([Bind("School_Id,School_Name,School_token,School_allowSite")] School school)
        {
            if (ModelState.IsValid)
            {
                _context.Add(school);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(school);
        }


        // POST: School/Edit/5
        [HttpPost("{id}")]
        public async Task<IActionResult> Edit(int id, [Bind("School_Id,School_Name,School_token,School_allowSite")] School school)
        {
            if (id != school.School_Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(school);
                    await _context.SaveChangesAsync();
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
                return RedirectToAction(nameof(Index));
            }
            return View(school);
        }

        // GET: School/Delete/5
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

            return View(school);
        }
        private bool SchoolExists(int id)
        {
            return _context.Schools.Any(e => e.School_Id == id);
        }
    }
}
