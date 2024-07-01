using System.Security.Claims;
using API_GesSIgn.Models;
using API_GesSIgn.Models.Request;
using API_GesSIgn.Models.Response;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Controllers
{

    [Route("api/[controller]")]
    [ApiController]
    [RoleRequirement("Gestion Ecole")]
    public class TeacherController : ControllerBase
    {
        private readonly MonDbContext _context;

        public TeacherController(MonDbContext context)
        {
            _context = context;

        }

        // GET: api/User
        [HttpGet]
        public async Task<ActionResult<IEnumerable<UserSimplifyDto>>> GetTeacher()
        {
            var schoolIdClaim = User.FindFirst("SchoolId")?.Value;
            if (string.IsNullOrEmpty(schoolIdClaim))
            {
                return BadRequest("School ID not found in token.");
            }

            // Return users from the same school
            var users = await _context.Users
                .Include(u => u.User_Role)
                .Include(u => u.User_School)
                .Where(u => u.User_Role.Role_Name == "Professeur" && u.User_School_Id == int.Parse(schoolIdClaim))
                .ToListAsync();

            var result = users.Select(sh => new UserSimplifyDto
            {
                User_Id = sh.User_Id,
                User_firstname = sh.User_firstname,
                User_lastname = sh.User_lastname,
                User_email = sh.User_email,
                User_num = sh.User_num,
            });
            
            return Ok(result);

        }

        [HttpPost("registerTeacher")]
        [RoleRequirement("Gestion Ecole")]
        public async Task<IActionResult> RegisterTeacher([FromBody] RegisterTeacherRequest request)
        {
            if (_context.Users.Any(u => u.User_email == request.User_email))
            {
                return BadRequest("Cet email est déjà utilisé.");
            }

            var role = _context.Roles.FirstOrDefault(r => r.Role_Name == "Professeur");
            if (role == null)
            {
                return NotFound("Le rôle 'teacher' n'existe pas.");
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

            return Ok("Enregistrement réussi.");
        }
    }

    /*[HttpGet]
    public async Task<ActionResult<IEnumerable<User>>> GetTeachers()
    {
        return await _context.Users.ToListAsync();
    }
    $*/
}
