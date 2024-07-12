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
using API_GesSIgn.Models.Response;

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
        [RoleRequirement("Gestion Ecole")]
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
        [RoleRequirement("Gestion Ecole")]
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

        [RoleRequirement("Gestion Ecole")]
        [HttpPost("registerStudent")]
        public async Task<IActionResult> RegisterStudent([FromBody] RegisterStudentRequest request)
        {
            if (_context.Users.Any(u => u.User_email == request.User_email))
            {
                return BadRequest("Cet email est déjà utilisé.");
            }

            var role = _context.Roles.FirstOrDefault(r => r.Role_Name == "Eleve");
            if (role == null)
            {
                return NotFound("Le rôle 'student' n'existe pas.");
            }

            var user = new User
            {
                User_email = request.User_email,
                User_password = request.User_password,
                User_Role = role,
                User_lastname = request.User_lastname,
                User_firstname = request.User_firstname,
                User_num = request.User_num,
                User_School_Id = request.User_School_Id
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            var student = new Student
            {
                Student_User_Id = user.User_Id,
                Student_Sector_Id = request.Student_Sector_Id
            };

            _context.Students.Add(student);
            await _context.SaveChangesAsync();

            return Ok("Enregistrement réussi.");
        }
    

        [HttpPut("{id}")]
        [RoleRequirement("Gestion Ecole")]
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


        [HttpGet("GetStudentByToken/")]
        [RoleRequirement("Eleve")]
        public async Task<ActionResult<IEnumerable<StudentSimplifyDto>>> GetStudentByToken()
        {
            
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            var student = await _context.Students
                .Include(s => s.Student_User)
                .Include(s => s.Student_Sectors)
                .FirstOrDefaultAsync(s => s.Student_User_Id == userId);

            if (student == null)
            {
                return NotFound();
            } else
            {
                return Ok(StudentSimplifyDto.FromStudent(student));
            }

            
        }

        /// <summary>
        /// Etudiant par école 
        /// </summary>
        /// <param name="NameClass">Option flitre par class</param>
        /// <returns></returns>
        [HttpGet("GetStudentsSchoolByToken/")]
        [RoleRequirement("Gestion Ecole")]
        public async Task<ActionResult<IEnumerable<StudentSimplifyDto>>> GetStudentsSchoolByToken(string? NameClass = null)
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
                List<Student> students = new List<Student>();
                if (NameClass != null)
                {
                     students = await _context.Students
                    .Include(s => s.Student_User)
                    .ThenInclude(u => u.User_School)
                    .Include(s => s.Student_Sectors)
                    .Where(s => s.Student_User.User_School.School_Name == nameSchool)
                    .ToListAsync();

                }
                else
                {
                     students = await _context.Students
                    .Include(s => s.Student_User)
                    .ThenInclude(u => u.User_School)
                    .Include(s => s.Student_Sectors)
                    .Where(s => s.Student_User.User_School.School_Name == nameSchool && s.Student_Sectors.Sectors_Name == NameClass)
                    .ToListAsync();
                    
                }

                List<StudentSimplifyDto> studentsSimplify = new List<StudentSimplifyDto>();
                foreach (var student in students)
                {
                    if (student.Student_Sectors.Sectors_Name == NameClass)
                    {
                        studentsSimplify.Add(StudentSimplifyDto.FromStudent(student));
                    }
                }

                return Ok(studentsSimplify);

            }
            else
            {
                return new ObjectResult("Vous ne possédez pas les droits pour voir la liste.") { StatusCode = 403 };
            }
        }

       

    }
}
