using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using API_GesSIgn.Models;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace API_GesSIgn.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly MonDbContext _context;

        public AuthController(MonDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Méthode pour l'enregistrement des utilisateurs
        /// </summary>
        /// <param name="registerUser"></param>
        /// <returns></returns>
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] User registerUser)
        {
            if (_context.Users.Any(u => u.User_email == registerUser.User_email))
                return BadRequest("Cet email est déjà utilisé.");

            var user = new User
            {
                User_email = registerUser.User_email,
                User_password = registerUser.User_password,
                User_Role = registerUser.User_Role,
                User_School_Id = registerUser.User_School_Id, 
                User_lastname = registerUser.User_lastname,
                User_firstname = registerUser.User_firstname,
                User_num = registerUser.User_num
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return Ok("Inscription réussie.");
        }

        /// <summary>
        /// Méthode pour la connexion des utilisateurs (Login)
        /// </summary>
        /// <param name="login"></param>
        /// <returns></returns>
        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginRequest login)
        {
            var user = _context.Users
                .Include(u => u.User_Role)
                .Include(u => u.User_School)
                .SingleOrDefault(u => u.User_email == login.User_email && u.User_password == login.User_password);

            if (user == null)
                return Unauthorized();

            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes("VotreCléSécrèteSuperSécuriséeDe32CaractèresOuPlus");
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.NameIdentifier, user.User_Id.ToString()),
                    new Claim(ClaimTypes.Name, user.User_firstname + " " + user.User_lastname),
                    new Claim(ClaimTypes.Role, user.User_Role.Role_Name),
                    new Claim("SchoolName", user.User_School?.School_Name ?? string.Empty)
                }),
                Expires = DateTime.UtcNow.AddHours(1),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            var tokenString = tokenHandler.WriteToken(token);

            return Ok(new { Token = tokenString });

        }
    }
}
