using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API_GesSIgn.Models;

namespace API_GesSIgn.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PresenceController : ControllerBase
    {
        private readonly MonDbContext _context;

        public PresenceController(MonDbContext context)
        {
            _context = context;
        }

        // GET: api/Presence
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Presence>>> GetPresences()
        {
            return await _context.Presences
                .Include(p => p.Presence_Student)
                .Include(p => p.Presence_SubjectsHour)
                .ThenInclude(sh => sh.SubjectsHour_Sectors)
                .ToListAsync();
        }

        // GET: api/Presence/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Presence>> GetPresence(int id)
        {
            var presence = await _context.Presences
                .Include(p => p.Presence_Student)
                .Include(p => p.Presence_SubjectsHour)
                .ThenInclude(sh => sh.SubjectsHour_Sectors)
                .FirstOrDefaultAsync(p => p.Presence_Id == id);

            if (presence == null)
            {
                return NotFound();
            }

            return presence;
        }

        // POST: api/Presence
        [HttpPost]
        public async Task<ActionResult<Presence>> PostPresence(Presence presence)
        {
            _context.Presences.Add(presence);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetPresence", new { id = presence.Presence_Id }, presence);
        }

        // PUT: api/Presence/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutPresence(int id, Presence presence)
        {
            if (id != presence.Presence_Id)
            {
                return BadRequest();
            }

            _context.Entry(presence).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!PresenceExists(id))
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

        // DELETE: api/Presence/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePresence(int id)
        {
            var presence = await _context.Presences.FindAsync(id);
            if (presence == null)
            {
                return NotFound();
            }

            _context.Presences.Remove(presence);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool PresenceExists(int id)
        {
            return _context.Presences.Any(e => e.Presence_Id == id);
        }
    }
}
