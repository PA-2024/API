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
                Subject = SubjectsdDto.FromSubjects(p.Presence_SubjectsHour.SubjectsHour_Subjects),
            }).ToList();

            return Ok(result);
        }

        /// <summary>
        /// Méthode pour récupérer les présences non confirmées (Presence_Is = false) pour un étudiant basé sur le token.
        /// </summary>
        /// <returns></returns>
        [HttpGet("OneDay")]
        [RoleRequirement("Eleve")]
        public async Task<ActionResult<IEnumerable<SubjectsHourSimplyWithPrescense>>> GetalllPresences()
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);

            var dayDate = DateTime.UtcNow;
            var dayStart = new DateTime(dayDate.Year, dayDate.Month, dayDate.Day, 0, 0, 0);
            var dayEnd = new DateTime(dayDate.Year, dayDate.Month, dayDate.Day, 23, 59, 59);


            var student = await _context.Students.FirstOrDefaultAsync(s => s.Student_User_Id == userId);
            if (student == null)
            {
                return NotFound("Student not found.");
            }

            var presences = await _context.Presences
                .Include(p => p.Presence_SubjectsHour)
                .ThenInclude(sh => sh.SubjectsHour_Subjects)
                .ThenInclude(s => s.Subjects_User)
                .Where(p => p.Presence_Student_Id == student.Student_Id &&
                            p.Presence_SubjectsHour.SubjectsHour_DateEnd <= dayEnd && p.Presence_SubjectsHour.SubjectsHour_DateStart >= dayStart)
                .ToListAsync();

            var result = presences.Select(p => new SubjectsHourSimplyWithPrescense
            {
                SubjectsHour_Id = p.Presence_SubjectsHour.SubjectsHour_Id,
                SubjectsHour_DateStart = p.Presence_SubjectsHour.SubjectsHour_DateStart,
                StudentIsPresent = p.Presence_Is,
                SubjectsHour_Subject = SubjectsdDto.FromSubjects(p.Presence_SubjectsHour.SubjectsHour_Subjects),
            }).ToList();

            return Ok(result);
        }


        /// <summary>
        /// Méthode pour récupérer le résumé des présences pour un étudiant basé sur le token.
        /// </summary>
        /// <returns></returns>
        [HttpGet("attendance-summary")]
        [RoleRequirement("Eleve")]
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
                .Where(p => p.Presence_Student_Id == student.Student_Id && p.Presence_ProofAbsence_Id == null)
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




        // GET: api/Presences/SubjectsHour/5
        [RoleRequirement("Professeur")]
        [HttpGet("SubjectsHourWithPresences/{id}")]
        public async Task<ActionResult<SubjectsHourDetailsWithStudentsDto>> GetSubjectsHourWithStudents(int id)
        {
            var subjectsHour = await _context.SubjectsHour
                .Include(sh => sh.SubjectsHour_Bulding)
                .Include(sh => sh.SubjectsHour_Subjects)
                    .ThenInclude(s => s.Subjects_User)
                .FirstOrDefaultAsync(sh => sh.SubjectsHour_Id == id);

            if (subjectsHour == null)
            {
                return NotFound();
            }

            var students = await _context.StudentSubjects
                .Where(ss => ss.StudentSubject_SubjectId == subjectsHour.SubjectsHour_Subjects.Subjects_Id)
                .Include(ss => ss.StudentSubject_Student)
                    .ThenInclude(s => s.Student_User)
                .Select(ss => new StudentIsPresent
                {
                    Student_Id = ss.StudentSubject_Student.Student_Id,
                    Student_User = new UserSimplifyDto
                    {
                        User_Id = ss.StudentSubject_Student.Student_User.User_Id,
                        User_email = ss.StudentSubject_Student.Student_User.User_email,
                        User_lastname = ss.StudentSubject_Student.Student_User.User_lastname,
                        User_firstname = ss.StudentSubject_Student.Student_User.User_firstname,
                        User_num = ss.StudentSubject_Student.Student_User.User_num
                    },
                    IsPresent = _context.Presences.Any(p => p.Presence_Student_Id == ss.StudentSubject_Student.Student_Id && p.Presence_SubjectsHour_Id == id && p.Presence_Is)
                })
                .ToListAsync();

            var result = new SubjectsHourDetailsWithStudentsDto
            {
                SubjectsHour_Id = subjectsHour.SubjectsHour_Id,
                SubjectsHour_DateStart = subjectsHour.SubjectsHour_DateStart,
                SubjectsHour_DateEnd = subjectsHour.SubjectsHour_DateEnd,
                SubjectsHour_Room = subjectsHour.SubjectsHour_Room,
                Subject = SubjectsdDto.FromSubjects(subjectsHour.SubjectsHour_Subjects),
                Students = students
            };

            return Ok(result);
        }




        private bool PresenceExists(int id)
        {
            return _context.Presences.Any(e => e.Presence_Id == id);
        }
    }
}
