using API_GesSIgn.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API_GesSIgn.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class SectorsController : ControllerBase
    {
        private readonly MonDbContext _context;

        public SectorsController(MonDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllSectors()
        {
            var sectors = await _context.Sectors
                                        .Include(s => s.Sectors_School)
                                        .Select(s => new 
                                        {
                                            s.Sectors_Id,
                                            s.Sectors_Name,
                                            Sectors_School = s.Sectors_School
                                        })
                                        .ToListAsync();

            return Ok(sectors);
        }


        // GET: Sectors/Details/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetSectorDetails(int id)
        {
            var sector = await _context.Sectors
                                        .Include(s => s.Sectors_School)
                                        .Select(s => new 
                                        {
                                            s.Sectors_Id,
                                            s.Sectors_Name,
                                            Sectors_School = s.Sectors_School
                                        })
                                        .FirstOrDefaultAsync(s => s.Sectors_Id == id);
            if (sector == null)
            {
                return NotFound();
            }

            return Ok(sector); 
        }

        // POST: Sectors/Create
        [HttpPost]
        public async Task<IActionResult> CreateSector([FromBody] Sectors sector)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var school = await _context.Schools.FindAsync(sector.Sectors_School_Id);
            if (school == null)
            {
                return BadRequest("School not found.");
            }
            sector.Sectors_School = school;

            _context.Sectors.Add(sector);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetSectorDetails), new { id = sector.Sectors_Id }, sector);
        }


        // PUT: Sectors/Edit/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateSector(int id, [FromBody] Sectors sector)
        {
            if (id != sector.Sectors_Id)
            {
                return BadRequest("Sector ID mismatch");
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _context.Entry(sector).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!SectorExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // DELETE: Sectors/Delete/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteSector(int id)
        {
            var sector = await _context.Sectors.FindAsync(id);
            if (sector == null)
            {
                return NotFound();
            }

            _context.Sectors.Remove(sector);
            await _context.SaveChangesAsync();
            return NoContent(); 
        }

        private bool SectorExists(int id)
        {
            return _context.Sectors.Any(s => s.Sectors_Id == id);
        }
    }
}
