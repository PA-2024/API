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
            
            return Ok(users);

        }

        /*[HttpGet]
        public async Task<ActionResult<IEnumerable<User>>> GetTeachers()
        {
            return await _context.Users.ToListAsync();
        }
        $*/
    }
}