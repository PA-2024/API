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

        // GET: Sectors
        [HttpGet]
        public async Task<IActionResult> GetAllSectors()
        {
            var sectors = await _context.Sectors.ToListAsync();
            return Ok(sectors); // Returns JSON list of sectors
        }

        // GET: Sectors/Details/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetSectorDetails(int id)
        {
            var sector = await _context.Sectors
                .FirstOrDefaultAsync(s => s.Sectors_Id == id);
            if (sector == null)
            {
                return NotFound();
            }

            return Ok(sector); // Returns JSON object of a single sector
        }

        // POST: Sectors/Create
        [HttpPost]
        public async Task<IActionResult> CreateSector([FromBody] Sectors sector)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

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

            return NoContent(); // Successfully updated with no content return
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
            return NoContent(); // Successful response with no content
        }

        private bool SectorExists(int id)
        {
            return _context.Sectors.Any(s => s.Sectors_Id == id);
        }
    }
}
