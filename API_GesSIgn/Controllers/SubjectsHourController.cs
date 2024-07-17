using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API_GesSIgn.Models;
using API_GesSIgn.Models.Request;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using API_GesSIgn.Models.Response;

namespace API_GesSIgn.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    
    public class SubjectsHourController : ControllerBase
    {
        private readonly MonDbContext _context;

        public SubjectsHourController(MonDbContext context)
        {
            _context = context;
        }

        // GET: api/SubjectsHour
        [HttpGet]
        [RoleRequirement(["Gestion Ecole", "Professeur"])]
        public async Task<ActionResult<IEnumerable<SubjectsHour>>> GetSubjectsHours()
        {
            var schoolIdClaim = User.FindFirst("SchoolId")?.Value;
            if (string.IsNullOrEmpty(schoolIdClaim))
            {
                return BadRequest("School ID not found in token.");
            }

            return await _context.SubjectsHour
                .Include(s => s.SubjectsHour_Subjects)
                .Where(s => s.SubjectsHour_Subjects.Subjects_School_Id == int.Parse(schoolIdClaim))
                .ToListAsync();
        }

        // GET: api/SubjectsHour/5
        [HttpGet("{id}")]
        [RoleRequirement("Gestion Ecole")]
        public async Task<ActionResult<SubjectsHour>> GetSubjectsHour(int id)
        {

            var schoolIdClaim = User.FindFirst("SchoolId")?.Value;
            if (string.IsNullOrEmpty(schoolIdClaim))
            {
                return BadRequest("School ID not found in token.");
            }
            var subjectsHour = await _context.SubjectsHour
                .Include(s => s.SubjectsHour_Subjects)
                .Where(s => s.SubjectsHour_Subjects.Subjects_School_Id == int.Parse(schoolIdClaim))
                .FirstOrDefaultAsync(s => s.SubjectsHour_Id == id);

            if (subjectsHour == null)
            {
                return NotFound();
            }

            return subjectsHour;
        }

        [HttpPost]
        public async Task<ActionResult<SubjectsHour>> PostSubjectsHour([FromBody] CreateSubjectHourRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var subject = await _context.Subjects.FindAsync(request.SubjectsHour_Subjects_Id);
            if (subject == null)
            {
                return NotFound($"Subject with ID {request.SubjectsHour_Subjects_Id} not found.");
            }

            var building = await _context.Buildings.FindAsync(request.SubjectsHour_Building_Id);
            if (building == null)
            {
                return NotFound($"Building with ID {request.SubjectsHour_Building_Id} not found.");
            }

            SubjectsHour subjectsHour = new SubjectsHour
            {
                SubjectsHour_DateStart = request.SubjectsHour_DateStart,
                SubjectsHour_DateEnd = request.SubjectsHour_DateEnd,
                SubjectsHour_Room = request.SubjectsHour_Room,
                SubjectsHour_Subjects = subject,
                SubjectsHour_Bulding = building
            };

            _context.SubjectsHour.Add(subjectsHour);
            await _context.SaveChangesAsync();

            // Créer des présences pour chaque étudiant inscrit au cours
            var studentsEnrolled = await _context.StudentSubjects
                .Where(ss => ss.StudentSubject_SubjectId == subject.Subjects_Id)
                .Select(ss => ss.StudentSubject_StudentId)
                .ToListAsync();

            foreach (var studentId in studentsEnrolled)
            {
                Presence presence = new Presence
                {
                    Presence_Student_Id = studentId,
                    Presence_SubjectsHour_Id = subjectsHour.SubjectsHour_Id,
                    Presence_Is = false 
                };

                _context.Presences.Add(presence);
            }

            await _context.SaveChangesAsync();

            return CreatedAtAction("GetSubjectsHour", new { id = subjectsHour.SubjectsHour_Id }, subjectsHour);
        }

        // PUT: api/SubjectsHour/5
        [HttpPut("{id}")]
        [RoleRequirement("Gestion Ecole")]
        public async Task<IActionResult> PutSubjectsHour(int id, SubjectsHour subjectsHour)
        {
            if (id != subjectsHour.SubjectsHour_Id)
            {
                return BadRequest();
            }

            _context.Entry(subjectsHour).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!SubjectsHourExists(id))
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

        // DELETE: api/SubjectsHour/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteSubjectsHour(int id)
        {
            var subjectsHour = await _context.SubjectsHour.FindAsync(id);
            if (subjectsHour == null)
            {
                return NotFound();
            }

            _context.SubjectsHour.Remove(subjectsHour);
            await _context.SaveChangesAsync();

            return NoContent();
        }


        /// <summary>
        /// Méthode pour récupérer les SubjectsHour pour un étudiant basé sur le token
        /// </summary>
        /// <param name="dateRange"></param>
        /// <returns></returns>
        [HttpGet("byDateRange")]
        [RoleRequirement("Eleve")]
        public async Task<ActionResult<IEnumerable<SubjectsHourDetailsDto>>> GetSubjectsHourByDateRange([FromQuery] DateRangeRequest dateRange)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);

            var student = await _context.Students.FirstOrDefaultAsync(s => s.Student_User_Id == userId);
            if (student == null)
            {
                return NotFound("Student not found.");
            }

            var subjectsHours = await _context.SubjectsHour
                .Include(sh => sh.SubjectsHour_Bulding)
                .Include(sh => sh.SubjectsHour_Subjects)
                .ThenInclude(s => s.Subjects_User)
                .Where(sh => sh.SubjectsHour_DateStart >= dateRange.StartDate && sh.SubjectsHour_DateEnd <= dateRange.EndDate)
                .Where(sh => _context.StudentSubjects.Any(ss => ss.StudentSubject_SubjectId == sh.SubjectsHour_Subjects_Id && ss.StudentSubject_StudentId == student.Student_Id))
                .ToListAsync();

            var result = subjectsHours.Select(sh => new SubjectsHourDetailsDto
            {
                SubjectsHour_Id = sh.SubjectsHour_Id,
                SubjectsHour_DateStart = sh.SubjectsHour_DateStart,
                SubjectsHour_DateEnd = sh.SubjectsHour_DateEnd,
                SubjectsHour_Room = sh.SubjectsHour_Room,
                SubjectsHour_TeacherComment = sh.SubjectsHour_TeacherComment,
                Building = BuildingDto.FromBuilding(sh.SubjectsHour_Bulding),
                Subject = SubjectsdDto.FromSubjects(sh.SubjectsHour_Subjects),
            }).ToList();

            return Ok(result);
        }


        /// <summary>
        /// Méthode pour récupérer les SubjectsHour pour un étudiant basé sur l'ID de l'étudiant
        /// </summary>
        /// <param name="studentId"></param>
        /// <param name="dateRange"></param>
        /// <returns></returns>
        [HttpGet("byDateRange/{studentId}")]
        [RoleRequirement("Eleve")]
        public async Task<ActionResult<IEnumerable<SubjectsHour>>> GetSubjectsHourByStudentIdAndDateRange(int studentId, [FromQuery] DateRangeRequest dateRange)
        {
            var student = await _context.Students.FindAsync(studentId);
            if (student == null)
            {
                return NotFound("Student not found.");
            }
            var schoolIdClaim = User.FindFirst("SchoolId")?.Value;
            if (string.IsNullOrEmpty(schoolIdClaim))
            {
                return BadRequest("School ID not found in token.");
            }

            var subjectsHours = await _context.SubjectsHour
                 .Include(sh => sh.SubjectsHour_Bulding)
                 .Include(sh => sh.SubjectsHour_Subjects)
                 .ThenInclude(s => s.Subjects_User)
                 .Where(sh => sh.SubjectsHour_DateStart >= dateRange.StartDate && sh.SubjectsHour_DateEnd <= dateRange.EndDate)
                 .Where(sh => _context.StudentSubjects.Any(ss => ss.StudentSubject_SubjectId == sh.SubjectsHour_Subjects_Id && ss.StudentSubject_StudentId == student.Student_Id))
                 .ToListAsync();

            var result = subjectsHours.Select(sh => new SubjectsHourDetailsDto
            {
                SubjectsHour_Id = sh.SubjectsHour_Id,
                SubjectsHour_DateStart = sh.SubjectsHour_DateStart,
                SubjectsHour_DateEnd = sh.SubjectsHour_DateEnd,
                SubjectsHour_Room = sh.SubjectsHour_Room,
                Building = BuildingDto.FromBuilding(sh.SubjectsHour_Bulding),
                Subject = SubjectsdDto.FromSubjects(sh.SubjectsHour_Subjects),
            }).ToList();

            return Ok(result);
        }
        
        // GET: api/SubjectsHour/Teacher/byDateRange
        [HttpGet("Teacher/byDateRange")]
        [RoleRequirement("Professeur")]
        public async Task<ActionResult<IEnumerable<SubjectsHourSimplify>>> GetSubjectsHourByTeacherAndDateRange([FromQuery] DateRangeRequest dateRange)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            var schoolIdClaim = User.FindFirst("SchoolId")?.Value;
            if (string.IsNullOrEmpty(schoolIdClaim))
            {
                return BadRequest("School ID not found in token.");
            }

            var subjectsHours = await _context.SubjectsHour
                .Include(sh => sh.SubjectsHour_Bulding)
                .Include(sh => sh.SubjectsHour_Subjects)
                .Where(sh => sh.SubjectsHour_Subjects.Subjects_User_Id == userId &&
                            sh.SubjectsHour_DateStart >= dateRange.StartDate && 
                            sh.SubjectsHour_DateEnd <= dateRange.EndDate && sh.SubjectsHour_Bulding.Bulding_School.School_Id == Convert.ToInt32(schoolIdClaim))
                .ToListAsync();
            try {
                var result = subjectsHours.Select(sh => SubjectsHourSimplify.FromSubjectsHour(sh)).ToList();
                return Ok(result);
            }
            catch (Exception ex) {
                return StatusCode(500, "Une erreur s'est produite lors de la récupération des cours." + ex.Message);
            }
        }

        [HttpGet("Class/byDateRange/{ClasseName}")]
        [RoleRequirement(["Professeur, Gestion Ecole"])]
        public async Task<ActionResult<IEnumerable<SubjectsHourSimplify>>> GetSubjectsHourByClassAndDateRange(string ClasseName ,[FromQuery] DateRangeRequest dateRange)
        {
            return Ok();
           
        }

        private bool SubjectsHourExists(int id)
        {
            return _context.SubjectsHour.Any(e => e.SubjectsHour_Id == id);
        }
    }
}
