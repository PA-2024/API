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

namespace API_GesSIgn.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
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
            var subjects = await _context.Subjects
                .Include(s => s.Subjects_User)
                .ToListAsync();

            var subjectDtos = subjects.Select(s => new SubjectsdDto
            {
                Subjects_Id = s.Subjects_Id,
                Subjects_Name = s.Subjects_Name,
                Teacher = new UserSimplifyDto
                {
                    User_Id = s.Subjects_User.User_Id,
                    User_email = s.Subjects_User.User_email,
                    User_lastname = s.Subjects_User.User_lastname,
                    User_firstname = s.Subjects_User.User_firstname,
                    User_num = s.Subjects_User.User_num
                }
            }).ToList();

            return Ok(subjectDtos);
        }

        // GET: api/Subjects/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Subjects>> GetSubjects(int id)
        {
            var subjects = await _context.Subjects
                .Include(s => s.Subjects_User)
                .FirstOrDefaultAsync(s => s.Subjects_Id == id);

            if (subjects == null)
            {
                return NotFound();
            }

            return subjects;
        }

        // POST: api/Subjects
        [HttpPost]
        public async Task<ActionResult<Subjects>> PostSubjects(Subjects subjects)
        {
            _context.Subjects.Add(subjects);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetSubjects", new { id = subjects.Subjects_Id }, subjects);
        }

        // PUT: api/Subjects/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutSubjects(int id, Subjects subjects)
        {
            if (id != subjects.Subjects_Id)
            {
                return BadRequest();
            }

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
