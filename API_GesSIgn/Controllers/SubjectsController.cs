using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using API_GesSIgn.Models;
using API_GesSIgn.Models.Response;
using API_GesSIgn.Models.Request;

namespace API_GesSIgn.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [RoleRequirement("Gestion Ecole")]
    public class SubjectsController : ControllerBase
    {
        private readonly MonDbContext _context;

        public SubjectsController(MonDbContext context)
        {
            _context = context;
        }

        // GET: api/Subjects
        [HttpGet]
        public async Task<ActionResult<IEnumerable<SubjectsdDto>>> GetSubjects()
        {
            var schoolIdClaim = User.FindFirst("SchoolId")?.Value;
            if (string.IsNullOrEmpty(schoolIdClaim))
            {
                return BadRequest("School ID not found in token.");
            }

            var subjects = await _context.Subjects
                .Include(s => s.Subjects_User)
                .Where(s => s.Subjects_School_Id == int.Parse(schoolIdClaim))
                .ToListAsync();

            var subjectDtos = subjects.Select(s => new SubjectsdDto
            {
                Subjects_Id = s.Subjects_Id,
                Subjects_Name = s.Subjects_Name,
                Teacher = UserSimplifyDto.FromUser(s.Subjects_User)
            }).ToList();

            return Ok(subjectDtos);
        }

        /// <summary>
        /// GET: api/Subjects/5
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id}")]
        public async Task<ActionResult<SubjectDetailsDto>> GetSubjects(int id)
        {
            var schoolIdClaim = User.FindFirst("SchoolId")?.Value;
            if (string.IsNullOrEmpty(schoolIdClaim))
            {
                return BadRequest("School ID not found in token.");
            }

            var subjects = await _context.Subjects
                .Include(s => s.Subjects_User)
                .Where(s => s.Subjects_School_Id == int.Parse(schoolIdClaim))
                .FirstOrDefaultAsync(s => s.Subjects_Id == id);

            if (subjects == null)
            {
                return NotFound();
            }

            var studentSubjects = await _context.StudentSubjects
                .Include(ss => ss.StudentSubject_Student)
                    .ThenInclude(student => student.Student_User)
                .Include(ss => ss.StudentSubject_Student)
                    .ThenInclude(student => student.Student_Sectors)
                .Where(ss => ss.StudentSubject_SubjectId == id)
                .ToListAsync();

            var studentDtos = studentSubjects.Select(ss => StudentSimplifyDto.FromStudent(ss.StudentSubject_Student)).ToList();

            var subjectDetails = new SubjectDetailsDto
            {
                Subjects_Id = subjects.Subjects_Id,
                Subjects_Name = subjects.Subjects_Name,
                Teacher = UserSimplifyDto.FromUser(subjects.Subjects_User),
                Students = studentDtos
            };

            return Ok(subjectDetails);
        }


        // POST: api/Subjects
        [HttpPost]
        public async Task<ActionResult<Subjects>> PostSubjects(CreateSubjectRequest request)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            var schoolIdClaim = User.FindFirst("SchoolId")?.Value;
            if (string.IsNullOrEmpty(schoolIdClaim))
            {
                return BadRequest("School ID not found in token.");
            }

            var user = await _context.Users.FindAsync(request.Subjects_User_Id);
            if (user == null)
            {
                return NotFound($"User with ID {request.Subjects_User_Id} not found.");
            }

            var subjects = new Subjects
            {
                Subjects_Name = request.Subjects_Name,
                Subjects_User = user,
                Subjects_School_Id = int.Parse(schoolIdClaim)
            };

            _context.Subjects.Add(subjects);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetSubjects", new { id = subjects.Subjects_Id }, subjects);
        }

        // PUT: api/Subjects/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutSubjects(int id, CreateSubjectRequest request)
        {
            if (id != request.Subjects_Id)
            {
                return BadRequest();
            }

            var subjects = await _context.Subjects
                .Include(s => s.Subjects_User)
                .FirstOrDefaultAsync(s => s.Subjects_Id == id);

            if (subjects == null)
            {
                return NotFound();
            }

            var user = await _context.Users.FindAsync(request.Subjects_User_Id);
            if (user == null)
            {
                return NotFound($"User with ID {request.Subjects_User_Id} not found.");
            }

            subjects.Subjects_Name = request.Subjects_Name;
            subjects.Subjects_User = user;

            // Assuming the School ID should not change, so not updating it here.

            _context.Entry(subjects).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!SubjectsExists(id))
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

        // DELETE: api/Subjects/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteSubjects(int id)
        {
            var subjects = await _context.Subjects.FindAsync(id);
            if (subjects == null)
            {
                return NotFound();
            }

            _context.Subjects.Remove(subjects);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        


        private bool SubjectsExists(int id)
        {
            return _context.Subjects.Any(e => e.Subjects_Id == id);
        }

        /*/ GET: api/SubjectsHour/byDateRange
        [HttpGet("byDateRange")]
        [Authorize]
        public async Task<ActionResult<IEnumerable<SubjectsHour>>> GetSubjectsHourByDateRange(DateTime startDate, DateTime endDate)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);

            var subjectsHours = await _context.SubjectsHour
                .Where(sh => sh.SubjectsHour_DateStart >= startDate && sh.SubjectsHour_DateEnd <= endDate)
                .Where(sh => _context.Students.Any(p => p.Presence_SubjectsHour_Id == sh.SubjectsHour_Id && p.Presence_User_Id == userId))
                .Include(sh => sh.SubjectsHour_Sectors)
                .ToListAsync();

            return Ok(subjectsHours);
        } */
    }
}
