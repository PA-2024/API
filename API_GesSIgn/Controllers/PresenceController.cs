using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API_GesSIgn.Models;
using API_GesSIgn.Models.Response;
using System.Security.Claims;

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
                .ThenInclude(sh => sh.SubjectsHour_Subjects)
                .ToListAsync();
        }

        // GET: api/Presence/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Presence>> GetPresence(int id)
        {
            var presence = await _context.Presences
                .Include(p => p.Presence_Student)
                .Include(p => p.Presence_SubjectsHour)
                .ThenInclude(sh => sh.SubjectsHour_Subjects)
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

        /// <summary>
        /// Méthode pour récupérer les présences non confirmées (Presence_Is = false) pour un étudiant basé sur le token.
        /// </summary>
        /// <returns></returns>
        [HttpGet("unconfirmed")]
        [RoleRequirement("Eleve")]
        public async Task<ActionResult<IEnumerable<SubjectsHourDetailsDto>>> GetUnconfirmedPresences()
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            var currentDateTime = DateTime.UtcNow.AddMinutes(15);

            var student = await _context.Students.FirstOrDefaultAsync(s => s.Student_User_Id == userId);
            if (student == null)
            {
                return NotFound("Student not found.");
            }

            var presences = await _context.Presences
                .Include(p => p.Presence_SubjectsHour)
                .ThenInclude(sh => sh.SubjectsHour_Subjects)
                .ThenInclude(s => s.Subjects_User)
                .Include(p => p.Presence_SubjectsHour.SubjectsHour_Bulding)
                .Where(p => p.Presence_Student_Id == student.Student_Id &&
                            !p.Presence_Is &&
                            p.Presence_SubjectsHour.SubjectsHour_DateEnd <= currentDateTime)
                .ToListAsync();

            var result = presences.Select(p => new SubjectsHourDetailsDto
            {
                SubjectsHour_Id = p.Presence_SubjectsHour.SubjectsHour_Id,
                SubjectsHour_DateStart = p.Presence_SubjectsHour.SubjectsHour_DateStart,
                SubjectsHour_DateEnd = p.Presence_SubjectsHour.SubjectsHour_DateEnd,
                SubjectsHour_Room = p.Presence_SubjectsHour.SubjectsHour_Room,
                Building = BuildingDto.FromBuilding(p.Presence_SubjectsHour.SubjectsHour_Bulding),
                Subject = new SubjectDetailsWithOutStudentSimplifyDto
                {
                    Subjects_Id = p.Presence_SubjectsHour.SubjectsHour_Subjects.Subjects_Id,
                    Subjects_Name = p.Presence_SubjectsHour.SubjectsHour_Subjects.Subjects_Name,
                    Teacher = UserSimplifyDto.FromUser(p.Presence_SubjectsHour.SubjectsHour_Subjects.Subjects_User)
                }
            }).ToList();

            return Ok(result);
        }


        /// <summary>
        /// Méthode pour récupérer le résumé des présences pour un étudiant basé sur le token.
        /// </summary>
        /// <returns></returns>
        [HttpGet("attendance-summary")]
        public async Task<ActionResult<AttendanceSummaryDto>> GetAttendanceSummary()
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);

            var student = await _context.Students.FirstOrDefaultAsync(s => s.Student_User_Id == userId);
            if (student == null)
            {
                return NotFound("Student not found.");
            }

            var presences = await _context.Presences
                .Include(p => p.Presence_SubjectsHour)
                .Where(p => p.Presence_Student_Id == student.Student_Id)
                .ToListAsync();

            var total_Present = presences.Count(p => p.Presence_Is);
            var total_Missed = presences.Count(p => !p.Presence_Is && p.Presence_SubjectsHour.SubjectsHour_DateEnd <= DateTime.UtcNow.AddMinutes(15));

            var summary = new AttendanceSummaryDto
            {
                Total_Present = total_Present,
                Total_Missed = total_Missed
            };

            return Ok(summary);
        }



        private bool PresenceExists(int id)
        {
            return _context.Presences.Any(e => e.Presence_Id == id);
        }
    }
}
