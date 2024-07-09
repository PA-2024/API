using API_GesSIgn.Helpers;
using API_GesSIgn.Models;
using API_GesSIgn.Models.Request;
using API_GesSIgn.Models.Response;
using Azure.Core;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace API_GesSIgn.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [RoleRequirement("Admin")]
    public class AdminController : Controller
    {
        private readonly MonDbContext _context;

        public AdminController(MonDbContext context)
        {
            _context = context;
        }

        [HttpPost("CreateAdmin")]
        public async Task<ActionResult<User>> CreateAdminSchool(UserRequest userRequest)
        {
            User user = new User();
            user.User_email = userRequest.User_email;
            user.User_num = userRequest.User_num;
            user.User_firstname = userRequest.User_firstname;
            user.User_lastname = userRequest.User_lastname;
            user.User_password = "test";

            var school = await _context.Schools
                .FirstOrDefaultAsync(m => m.School_Id == userRequest.user_school_id);
            if (school == null)
            {
                return NotFound(); 
            }

            var role = await _context.Roles
                .FirstOrDefaultAsync(m => m.Role_Name == "Gestion Ecole");
            if (role == null)
            {
                return NotFound("Role");
            }
            user.User_School = school;
            user.User_Role = role;

            _context.Users.Add(user);
            await _context.SaveChangesAsync();
            return Ok("User Gestion Ecole ajouté");

        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<User>> DeleteAdminSchool(int id)
        {
            var user = await _context.Users.FirstOrDefaultAsync(m => m.User_Id == id);
            if (user == null)
            {
                return NotFound();
            }
            _context.Users.Remove(user);
            await _context.SaveChangesAsync();


            return Ok("Suppresion éffectué");
        }

        [HttpGet]
        public async Task<ActionResult<User>> GetAdminSchool(int id)
        {
            var users = await _context.Users
                    .Include(u => u.User_Role)
                    .Include(u => u.User_School)
                    .Where(u => u.User_Role.Role_Name == "Gestion Ecole")
                    .ToListAsync();
            return Ok(users);
        }

        [HttpPatch("{id}")]
        public async Task<ActionResult<User>> PathAdminSchool(int id, UserRequest userRequest)
        {
            var user = await _context.Users.FirstOrDefaultAsync(m => m.User_Id == id);
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
}
