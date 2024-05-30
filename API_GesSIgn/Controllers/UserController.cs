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
                    .Include(u => u.User_School)
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
                    .Include(u => u.User_School)
                    .Where(u => u.User_School_Id == currentUser.User_School_Id)
                    .ToListAsync();
                return Ok(users);
            }
            else
            {
                return new ObjectResult("Vous ne possédez pas les droits pour voir la liste.") { StatusCode = 403 };
            }
        }

        // PUT: api/User/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateUser(int id, [FromBody] UserUpdateRequest updateUser)
        {
            var roleName = User.FindFirst(ClaimTypes.Role)?.Value;
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            
            if (roleName == null || userId == 0)
            {
                return StatusCode(500, "Erreur lors de la récupération des informations de l'utilisateur authentifié.");
            }

            // Récupérer l'utilisateur à mettre à jour
            var userToUpdate = await _context.Users.Include(u => u.User_Role).FirstOrDefaultAsync(u => u.User_Id == id);

            if (userToUpdate == null)
            {
                return NotFound("Utilisateur non trouvé.");
            }
            // L'utilisateur avec le rôle "Admin" peut modifier les utilisateurs avec le rôle "Gestion Ecole"
            if (roleName == "Admin" && userToUpdate.User_Role.Role_Name == "Gestion Ecole")
            {
                if (!string.IsNullOrEmpty(updateUser.User_email))
                    userToUpdate.User_email = updateUser.User_email;
                if (!string.IsNullOrEmpty(updateUser.User_lastname))
                    userToUpdate.User_lastname = updateUser.User_lastname;
                if (!string.IsNullOrEmpty(updateUser.User_firstname))
                    userToUpdate.User_firstname = updateUser.User_firstname;
                if (!string.IsNullOrEmpty(updateUser.User_num))
                    userToUpdate.User_num = updateUser.User_num;
            }
            // L'utilisateur avec le rôle "Gestion Ecole" peut modifier les utilisateurs de son école
            else if (roleName == "Gestion Ecole" && userToUpdate.User_School_Id == userId)
            {
                if (!string.IsNullOrEmpty(updateUser.User_email))
                    userToUpdate.User_email = updateUser.User_email;
                if (!string.IsNullOrEmpty(updateUser.User_lastname))
                    userToUpdate.User_lastname = updateUser.User_lastname;
                if (!string.IsNullOrEmpty(updateUser.User_firstname))
                    userToUpdate.User_firstname = updateUser.User_firstname;
                if (!string.IsNullOrEmpty(updateUser.User_num))
                    userToUpdate.User_num = updateUser.User_num;
            }
            else
            {
                // L'utilisateur n'a pas les autorisations nécessaires pour effectuer cette action
                return new ForbidResult();
            }
            try
            {
                await _context.SaveChangesAsync();
                return NoContent(); // Retourner un code 204
            }
            catch (DbUpdateException ex)
            {
                return StatusCode(500, "Une erreur s'est produite lors de la mise à jour de l'utilisateur." + ex.Message);
            }
        }


        // GET: api/User/bytoken/
        [HttpGet("bytoken/")]
        public async Task<ActionResult<IEnumerable<User>>> GetMyUsersByToken() {
            
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);

            var users = await _context.Users
                    .Include(u => u.User_Role)
                    .Include(u => u.User_School)
                    .Where(u => u.User_Id == userId)
                    .ToListAsync();
            return Ok(users);
        }

        // GET : api/User/Prof
        [HttpGet("Prof/")]
        public async Task<ActionResult<IEnumerable<User>>> GetProfByToken()
        {
            var roleName = User.FindFirst(ClaimTypes.Role)?.Value;
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);

            if (roleName == "Gestion Ecole")
            {
                var currentUser = await _context.Users
                    .Include(u => u.User_School)
                    .FirstOrDefaultAsync(u => u.User_Id == userId);
                if (currentUser == null || currentUser.User_School == null)
                {
                    return BadRequest("User or user's school not found.");
                }

                // Return users with the role "Gestion Ecole"
                var users = await _context.Users
                    .Include(u => u.User_Role)
                    .Where(u => u.User_Role.Role_Name == "Professeur")
                    .Where(u => u.User_School_Id == currentUser.User_School_Id)
                    .ToListAsync();
                return Ok(users);
            }
            else
            {
                return new ObjectResult("Vous ne possédez pas les droits pour voir la liste.") { StatusCode = 403 };
            }

        }


        public static bool UserExists(int id, MonDbContext _context)
        {
            return _context.Users.Any(e => e.User_Id == id);
        }   

    }
}
