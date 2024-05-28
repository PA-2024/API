using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using API_GesSIgn.Models;

namespace API_GesSIgn.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class UserController : ControllerBase
    {
        private readonly MonDbContext _context;

        public UserController(MonDbContext context)
        {
            _context = context;
        }

        // GET: api/User
        [HttpGet]
        public async Task<ActionResult<IEnumerable<User>>> GetUsersByRole()
        {
            var roleName = User.FindFirst(ClaimTypes.Role)?.Value;
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);

            if (roleName == "Admin")
            {
                // Return users with the role "Gestion Ecole"
                var users = await _context.Users
                    .Include(u => u.User_Role)
                    .Where(u => u.User_Role.Role_Name == "Gestion Ecole")
                    .ToListAsync();
                return Ok(users);
            }
            else if (roleName == "Gestion Ecole")
            {
                // Get the school of the current user
                var currentUser = await _context.Users
                    .Include(u => u.User_School)
                    .FirstOrDefaultAsync(u => u.User_Id == userId);

                if (currentUser == null || currentUser.User_School == null)
                {
                    return BadRequest("User or user's school not found.");
                }

                // Return users from the same school
                var users = await _context.Users
                    .Include(u => u.User_Role)
                    .Where(u => u.User_School_Id == currentUser.User_School_Id)
                    .ToListAsync();
                return Ok(users);
            }
            else
            {
                return new ObjectResult("Vous ne possédez pas les droits pour voir la liste.") { StatusCode = 403 };
            }
        }
    }
}
