using API_GesSIgn.Models;
using API_GesSIgn.Models.Request;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

/* 
    Créé le : 14 May 2024
    Créé par : NicolasDebras
    Modifications :
        0455995 - fix UpdateBuilding - debrasnicolas
    8c4e904 - fix CreateBuilding - NicolasDebras
    3b2eb80 - add GetBuildingbySchool - NicolasDebras
    df56d6e - update foreignkey - NicolasDebras
    4471112 - add building controllers - NicolasDebras
*/

namespace API_GesSIgn.Controllers
{
    [ApiController]
    [Route("[controller]")]
    [RoleRequirement("Gestion Ecole")]
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
            var schoolIdClaim = User.FindFirst("SchoolId")?.Value;
            if (string.IsNullOrEmpty(schoolIdClaim))
            {
                return BadRequest("School ID not found in token.");
            }
            var buildings = await _context.Buildings.Include(b => b.Bulding_School).Where(b => b.Bulding_School.School_Id == Convert.ToInt32(schoolIdClaim) )  .ToListAsync();
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

        // GET: Buildings/GetBySchool/5
        [HttpGet("GetBySchool/{id}")]
        [RoleRequirement("Gestion Ecole")]
        public async Task<IActionResult> GetBuildingbySchool(int id)
        {
            var buildings = await _context.Buildings
                .Include(b => b.Bulding_School)
                .Where(b => b.Bulding_School.School_Id == id).ToListAsync(); ;
            if (buildings == null)
            {
                return NotFound();
            }

            return Ok(buildings);
        }

        // POST: Buildings/Create
        [HttpPost]
        [RoleRequirement("Gestion Ecole")]
        public async Task<IActionResult> CreateBuilding([FromBody] CreateBuildingRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var schoolIdClaim = User.Claims.FirstOrDefault(c => c.Type == "SchoolId");
            if (schoolIdClaim == null || !int.TryParse(schoolIdClaim.Value, out int schoolId))
            {
                return Unauthorized("School ID not found in token.");
            }

            var school = await _context.Schools.FindAsync(schoolId);
            if (school == null)
            {
                return NotFound($"School with ID {schoolId} not found.");
            }

            var building = new Building
            {
                Bulding_City = request.Bulding_City,
                Bulding_Name = request.Bulding_Name,
                Bulding_Adress = request.Bulding_Adress,
                Bulding_School = school
            };

            _context.Buildings.Add(building);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetBuildingDetails), new { id = building.Bulding_Id }, building);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateBuilding(int id, [FromBody] UpdateBuildingRequest request)
        {
            var building = await _context.Buildings.FindAsync(id);
            if (building == null)
            {
                return NotFound();
            }

            building.Bulding_City = request.Bulding_City;
            building.Bulding_Name = request.Bulding_Name;
            building.Bulding_Adress = request.Bulding_Adress;

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
