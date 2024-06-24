using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API_GesSIgn.Models;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using System.Runtime.CompilerServices;
using API_GesSIgn.Models.Request;

namespace API_GesSIgn.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [RoleRequirement("Gestion Ecole")]
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
            var schoolIdClaim = User.FindFirst("SchoolId")?.Value;
            if (string.IsNullOrEmpty(schoolIdClaim))
            {
                return BadRequest("School ID not found in token.");
            }

            return await _context.Students
                .Include(s => s.Student_User)
                .Include(s => s.Student_Sectors)
                .Where(s => s.Student_User.User_School_Id == int.Parse(schoolIdClaim))
                .ToListAsync();
        }

        // GET: api/Student/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Student>> GetStudent(int id)
        {
            var schoolIdClaim = User.FindFirst("SchoolId")?.Value;
            if (string.IsNullOrEmpty(schoolIdClaim))
            {
                return BadRequest("School ID not found in token.");
            }

            var student = await _context.Students
                .Include(s => s.Student_User)
                .Include(s => s.Student_Sectors)
                .Where(s => s.Student_User.User_School_Id == int.Parse(schoolIdClaim))
                .FirstOrDefaultAsync(s => s.Student_Id == id);

            if (student == null)
            {
                return NotFound();
            }

            return student;
        }

        // POST: api/Student
        [HttpPost]
        public async Task<ActionResult<Student>> PostStudent(StudentRequest student)
        {
            if (student == null && UserController.UserExists(student.Student_User_id, _context) && SectorsController.SectorExist(student.Student_Class_id, _context) )
            {
                return BadRequest("Student is null or Student/id wrong ");
            }
            var result = new Student
            {
                Student_User_Id = student.Student_User_id,
                Student_Sector_Id = student.Student_Class_id
            };

            _context.Students.Add(result);
            var save = await _context.SaveChangesAsync();

            return CreatedAtAction("GetStudent", new { id = result.Student_Id }, student);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutStudent(int id, [FromBody] UpdateStudentSectorRequest request)
        {
            var student = await _context.Students.FindAsync(id);
            if (student == null)
            {
                return NotFound();
            }
            student.Student_Sector_Id = request.Student_Sector_Id;
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
