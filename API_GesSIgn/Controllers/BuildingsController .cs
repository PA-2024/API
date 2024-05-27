using API_GesSIgn.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API_GesSIgn.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class BuildingsController : ControllerBase
    {
        private readonly MonDbContext _context;

        public BuildingsController(MonDbContext context)
        {
            _context = context;
        }

        // GET: Buildings
        [HttpGet]
        public async Task<IActionResult> GetAllBuildings()
        {
            var buildings = await _context.Buildings.Include(b => b.Bulding_School).ToListAsync();
            return Ok(buildings);
        }

        // GET: Buildings/Details/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetBuildingDetails(int id)
        {
            var building = await _context.Buildings
                .Include(b => b.Bulding_School)
                .FirstOrDefaultAsync(b => b.Bulding_Id == id);
            if (building == null)
            {
                return NotFound();
            }

            return Ok(building);
        }

        // POST: Buildings/Create
        [HttpPost]
        public async Task<IActionResult> CreateBuilding([FromBody] Building building)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _context.Buildings.Add(building);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetBuildingDetails), new { id = building.Bulding_Id }, building);
        }

        // PUT: Buildings/Edit/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateBuilding(int id, [FromBody] Building building)
        {
            if (id != building.Bulding_Id)
            {
                return BadRequest("Building ID mismatch");
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _context.Entry(building).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!BuildingExists(id))
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

        // DELETE: Buildings/Delete/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteBuilding(int id)
        {
            var building = await _context.Buildings.FindAsync(id);
            if (building == null)
            {
                return NotFound();
            }

            _context.Buildings.Remove(building);
            await _context.SaveChangesAsync();
            return NoContent();
        }

        private bool BuildingExists(int id)
        {
            return _context.Buildings.Any(b => b.Bulding_Id == id);
        }
    }
}
