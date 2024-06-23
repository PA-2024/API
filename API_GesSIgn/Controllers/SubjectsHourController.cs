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
    [Authorize]
    public class SubjectsHourController : ControllerBase
    {
        private readonly MonDbContext _context;

        public SubjectsHourController(MonDbContext context)
        {
            _context = context;
        }

        // GET: api/SubjectsHour
        [HttpGet]
        public async Task<ActionResult<IEnumerable<SubjectsHour>>> GetSubjectsHours()
        {
            return await _context.SubjectsHour
                .Include(s => s.SubjectsHour_Subjects)
                .ToListAsync();
        }

        // GET: api/SubjectsHour/5
        [HttpGet("{id}")]
        public async Task<ActionResult<SubjectsHour>> GetSubjectsHour(int id)
        {
            var subjectsHour = await _context.SubjectsHour
                .Include(s => s.SubjectsHour_Subjects)
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
                Building = BuildingDto.FromBuilding(sh.SubjectsHour_Bulding),
                Subject = new SubjectDetailsWithOutStudentSimplifyDto
                {
                    Subjects_Id = sh.SubjectsHour_Subjects.Subjects_Id,
                    Subjects_Name = sh.SubjectsHour_Subjects.Subjects_Name,
                    Teacher = UserSimplifyDto.FromUser(sh.SubjectsHour_Subjects.Subjects_User)
                }
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
        public async Task<ActionResult<IEnumerable<SubjectsHour>>> GetSubjectsHourByStudentIdAndDateRange(int studentId, [FromQuery] DateRangeRequest dateRange)
        {
            var student = await _context.Students.FindAsync(studentId);
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
                Building = BuildingDto.FromBuilding(sh.SubjectsHour_Bulding),
                Subject = new SubjectDetailsWithOutStudentSimplifyDto
                {
                    Subjects_Id = sh.SubjectsHour_Subjects.Subjects_Id,
                    Subjects_Name = sh.SubjectsHour_Subjects.Subjects_Name,
                    Teacher = UserSimplifyDto.FromUser(sh.SubjectsHour_Subjects.Subjects_User)
                }
            }).ToList();

            return Ok(result);
        }
        
        private bool SubjectsHourExists(int id)
        {
            return _context.SubjectsHour.Any(e => e.SubjectsHour_Id == id);
        }
    }
}
