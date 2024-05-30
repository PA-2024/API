using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API_GesSIgn.Models;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using System.Runtime.CompilerServices;

namespace API_GesSIgn.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StudentController : ControllerBase
    {
        private readonly MonDbContext _context;

        public StudentController(MonDbContext context)
        {
            _context = context;
        }


        // GET: api/Student
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Student>>> GetStudents()
        {
            return await _context.Students
                .Include(s => s.Student_User)
                .Include(s => s.Student_Sectors)
                .ToListAsync();
        }

        // GET: api/Student/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Student>> GetStudent(int id)
        {
            var student = await _context.Students
                .Include(s => s.Student_User)
                .Include(s => s.Student_Sectors)
                .FirstOrDefaultAsync(s => s.Student_Id == id);

            if (student == null)
            {
                return NotFound();
            }

            return student;
        }

        // POST: api/Student
        [HttpPost]
        public async Task<ActionResult<Student>> PostStudent(Student student)
        {
            _context.Students.Add(student);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetStudent", new { id = student.Student_Id }, student);
        }

        // PUT: api/Student/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutStudent(int id, Student student)
        {
            if (id != student.Student_Id)
            {
                return BadRequest();
            }

            _context.Entry(student).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!StudentExists(id))
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

        // DELETE: api/Student/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteStudent(int id)
        {
            var student = await _context.Students.FindAsync(id);
            if (student == null)
            {
                return NotFound();
            }

            _context.Students.Remove(student);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool StudentExists(int id)
        {
            return _context.Students.Any(e => e.Student_Id == id);
        }

        [HttpGet("GetStudentsSchoolByToken/")]
        [Authorize]
        public async Task<ActionResult<IEnumerable<Student>>> GetStudentsSchoolByToken()
        {
            var roleName = User.FindFirst(ClaimTypes.Role)?.Value;
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);

            if (roleName == "Gestion Ecole")
            {
                string nameSchool = User.FindFirst("SchoolName")?.Value;

                if (string.IsNullOrEmpty(nameSchool))
                {
                    return BadRequest("Le nom de l'école n'est pas fourni.");
                }

                var students = await _context.Students
                    .Include(s => s.Student_User)
                    .ThenInclude(u => u.User_School)
                    .Include(s => s.Student_Sectors)
                    .Where(s => s.Student_User.User_School.School_Name == nameSchool)
                    .ToListAsync();

                return Ok(students); // Ajout de cette ligne pour retourner les étudiants
            }
            else
            {
                return new ObjectResult("Vous ne possédez pas les droits pour voir la liste.") { StatusCode = 403 };
            }
        }

    }
}
