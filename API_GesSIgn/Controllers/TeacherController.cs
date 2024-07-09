using System.Data;
using System.Security.Claims;
using API_GesSIgn.Models;
using API_GesSIgn.Models.Request;
using API_GesSIgn.Models.Response;
using Azure.Core;
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

        /// <summary>
        /// Supréssion d'un professeur
        /// </summary>
        /// <param name="Teacher_id"></param>
        /// <returns></returns>
        [HttpDelete("DeleteTeacher/{Teacher_id}")]
        [RoleRequirement("Gestion Ecole")]
        public async Task<IActionResult> DeleteTeacher(int Teacher_id)
        {

            var user = _context.Users.FirstOrDefault(u => u.User_Id == Teacher_id);
            if (user == null)
            {
                return NotFound("Le Professeur n'existe pas.");
            }

            var subject = _context.Subjects.FirstOrDefault(s => s.Subjects_User_Id == Teacher_id);
            if (user == null)
            {
                return NotFound("Le Professeur possède un ou plusieurs cours.");
            }


            _context.Users.Remove(user);
            await _context.SaveChangesAsync();

            return Ok("Supréssion réussi.");
        }

        [HttpPatch("{id}")]
        public async Task<ActionResult<User>> PathTeacher(int id, UserRequest userRequest)
        {
            var user = await _context.Users.Include(u => u.User_Role)
                    .Include(u => u.User_School).FirstOrDefaultAsync(m => m.User_Id == id);
            if (user == null)
            {
                return NotFound();
            }
            if (userRequest.User_email != userRequest.User_email)
                user.User_email = userRequest.User_email;
            if (userRequest.User_num != userRequest.User_num)
                user.User_num = userRequest.User_num;
            if (userRequest.User_firstname != userRequest.User_firstname)
                user.User_firstname = userRequest.User_firstname;
            if (userRequest.User_lastname != userRequest.User_lastname)
                user.User_lastname = userRequest.User_lastname;

            _context.Users.Update(user);
            await _context.SaveChangesAsync();

            return Ok(user);
        }


    }



    /*[HttpGet]
    public async Task<ActionResult<IEnumerable<User>>> GetTeachers()
    {
        return await _context.Users.ToListAsync();
    }
    $*/
}
