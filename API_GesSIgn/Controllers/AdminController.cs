using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using API_GesSIgn.Models;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using API_GesSIgn.Models.Response;
using API_GesSIgn.Helpers;
using API_GesSIgn.Models.Request;
using API_GesSIgn.Models.Response;
using Azure.Core;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using static System.Collections.Specialized.BitVector32;

namespace API_GesSIgn.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AdminController
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
                return NotFound("school");
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
            return Ok("Update éffectué");
        }

    }
}
