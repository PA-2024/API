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

            var schoolIdClaim = User.FindFirst("SchoolId")?.Value;
            if (string.IsNullOrEmpty(schoolIdClaim))
            {
                return BadRequest("School ID not found in token.");
            }

            return await _context.Presences
                .Include(p => p.Presence_Student)
                .Include(p => p.Presence_SubjectsHour)
                .ThenInclude(sh => sh.SubjectsHour_Subjects)
                .Where(p => p.Presence_SubjectsHour.SubjectsHour_Subjects.Subjects_School_Id == int.Parse(schoolIdClaim) 
                && p.Presence_SubjectsHour.SubjectsHour_DateEnd <= DateTime.UtcNow.AddMinutes(15))
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
                            p.Presence_ProofAbsence_Id == null &&  !p.Presence_Is &&
                            p.Presence_SubjectsHour.SubjectsHour_DateEnd <= currentDateTime 
                            )
                .ToListAsync();

            var result = presences.Select(p => new SubjectsHourDetailsDto
            {
                SubjectsHour_Id = p.Presence_SubjectsHour.SubjectsHour_Id,
                SubjectsHour_DateStart = p.Presence_SubjectsHour.SubjectsHour_DateStart,
                SubjectsHour_DateEnd = p.Presence_SubjectsHour.SubjectsHour_DateEnd,
                SubjectsHour_Room = p.Presence_SubjectsHour.SubjectsHour_Room,
                Presence_id = p.Presence_Id,
                Building = BuildingDto.FromBuilding(p.Presence_SubjectsHour.SubjectsHour_Bulding),
                Subject = SubjectsdDto.FromSubjects(p.Presence_SubjectsHour.SubjectsHour_Subjects),
            }).ToList();

            return Ok(result);
        }


        /// <summary>
        /// Méthode pour récupérer les présences ou il y a un justicafif d'abscence pour un étudiant basé sur le token.
        /// </summary>
        /// <returns></returns>
        [HttpGet("Check")]
        [RoleRequirement("Eleve")]
        public async Task<ActionResult<IEnumerable<SubjectsHourDetailsDtoSUserProof>>> GetProofPresences()
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            var currentDateTime = DateTime.UtcNow.AddMinutes(15);

            var student = await _context.Students.FirstOrDefaultAsync(s => s.Student_User_Id == userId);
            if (student == null)
            {
                return NotFound("Student not found.");
            }

            var presences = await _context.Presences
                .Include(p => p.Presence_ProofAbsence)
                .Include(p => p.Presence_SubjectsHour)
                .ThenInclude(sh => sh.SubjectsHour_Subjects)
                .ThenInclude(s => s.Subjects_User)
                .Include(p => p.Presence_SubjectsHour.SubjectsHour_Bulding)
                .Where(p => p.Presence_Student_Id == student.Student_Id &&
                            p.Presence_ProofAbsence_Id != null && !p.Presence_Is &&
                            p.Presence_SubjectsHour.SubjectsHour_DateEnd <= currentDateTime)
                .ToListAsync();

            var result = presences.Select(p => new SubjectsHourDetailsDtoSUserProof
            {
                SubjectsHour_Id = p.Presence_SubjectsHour.SubjectsHour_Id,
                SubjectsHour_DateStart = p.Presence_SubjectsHour.SubjectsHour_DateStart,
                SubjectsHour_DateEnd = p.Presence_SubjectsHour.SubjectsHour_DateEnd,
                SubjectsHour_Room = p.Presence_SubjectsHour.SubjectsHour_Room,
                Presence_id = p.Presence_Id,
                ProofAbsence =  ProofAbsenceResponse.FromProofAbsence(p.Presence_ProofAbsence),
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
                SubjectsHour_DateEnd = p.Presence_SubjectsHour.SubjectsHour_DateEnd,
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




        [RoleRequirement("Professeur")]
        [HttpGet("SubjectsHourWithPresences/{id}")]
        public async Task<ActionResult<SubjectsHourDetailsWithStudentsDto>> GetSubjectsHourWithStudents(int id)
        {
            var subjectsHour = await _context.SubjectsHour
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
                .ToListAsync();

            List<StudentIsPresent> studentIsPresents = new List<StudentIsPresent>();
            foreach (var student in students)
            {
                StudentIsPresent add = new StudentIsPresent();
                var p = await  _context.Presences
                    .Include(p => p.Presence_Student)
                    .ThenInclude(p => p.Student_User)
                    .Where(p => p.Presence_Student_Id == student.StudentSubject_Student.Student_Id && p.Presence_SubjectsHour_Id == id)
                    .FirstOrDefaultAsync();
                if (p == null)
                {
                    // si l'étudiant ne possede pas de presence, on en crée une
                    Presence presence = new Presence
                    {
                        Presence_Student_Id = student.StudentSubject_Student.Student_Id,
                        Presence_SubjectsHour_Id = id,
                        Presence_Is = false
                    };
                    _context.Presences.Add(presence);
                    await _context.SaveChangesAsync();
                    p = presence;
                }
                add.Student_User = UserSimplifyDto.FromUser(p.Presence_Student.Student_User);
                add.IsPresent = p.Presence_Is;
                add.Presence_id = p.Presence_Id;
                studentIsPresents.Add(add);
            }   

            var result = new SubjectsHourDetailsWithStudentsDto
            {
                SubjectsHour_Id = subjectsHour.SubjectsHour_Id,
                SubjectsHour_DateStart = subjectsHour.SubjectsHour_DateStart,
                SubjectsHour_DateEnd = subjectsHour.SubjectsHour_DateEnd,
                SubjectsHour_Room = subjectsHour.SubjectsHour_Room,
                Subject = SubjectsdDto.FromSubjects(subjectsHour.SubjectsHour_Subjects),
                Students = studentIsPresents
            };

            return Ok(result);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [HttpPatch("confirm/{id}")]
        [RoleRequirement("Professeur")]
        public async Task<IActionResult> CheckAbsence(int id)
        {
            var p = _context.Presences.FirstOrDefault(p => p.Presence_Id == id);
            if (p == null)
            {
                return NotFound();
            }
            p.Presence_Is = true;
            _context.Update(p);
            await _context.SaveChangesAsync();

            return StatusCode(201, "Présence valié");
        }



        private bool PresenceExists(int id)
        {
            return _context.Presences.Any(e => e.Presence_Id == id);
        }
    }
}
