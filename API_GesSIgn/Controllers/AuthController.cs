using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using API_GesSIgn.Models;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Services;

/* 
    Créé le : 21 May 2024
    Créé par : NicolasDebras
    Modifications :
        35090bd - update token  date Expires - NicolasDebras
    6377337 - fix bug UpdateUser (pour que @capdrake arrete de me faire chier ) - debrasnicolas
    9a7c845 - fix error - NicolasDebras
    73d2e25 - ajout UpdateUser pour le petit tdc de bastien - NicolasDebras
    94fa232 - update auth - NicolasDebras
    565490f - fix Login - NicolasDebras
    28f6c00 - add list - NicolasDebras
    e9d8433 - update register - debrasnicolas
    2145786 - update login - debrasnicolas
    b9dc9b6 - update controller auth - debrasnicolas
    e274b79 - add controller & ajout d' une ecole dans les utilisateurs - NicolasDebras
    a44dd9e - add auth work - NicolasDebras
*/


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
                    new Claim("SchoolName", user.User_School?.School_Name ?? string.Empty),
                    new Claim("SchoolId", user.User_School?.School_Id.ToString() ?? string.Empty)
                }),
                Expires = DateTime.UtcNow.AddDays(7),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            var tokenString = tokenHandler.WriteToken(token);

            return Ok(new { Token = "Bearer "  + tokenString });

        }

        /// <summary>
        /// Méthode pour la connexion des utilisateurs (Login)
        /// </summary>
        /// <param name="login"></param>
        /// <returns></returns>
        [HttpPost("student/login")]
        public IActionResult StudentLogin([FromBody] LoginRequest login)
        {
            var user = _context.Users
                .Include(u => u.User_Role)
                .Include(u => u.User_School)
                .SingleOrDefault(u => u.User_email == login.User_email && u.User_password == login.User_password 
                && u.User_Role.Role_Name == "Eleve");

            if (user == null)
                return Unauthorized();

            var student = _context.Students
                .FirstOrDefault(s => s.Student_User.User_Id == user.User_Id);

            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes("VotreCléSécrèteSuperSécuriséeDe32CaractèresOuPlus");
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.NameIdentifier, user.User_Id.ToString()),
                    new Claim(ClaimTypes.Name, user.User_firstname + " " + user.User_lastname),
                    new Claim(ClaimTypes.Role, user.User_Role.Role_Name),
                    new Claim("SchoolName", user.User_School?.School_Name ?? string.Empty),
                    new Claim("SchoolId", user.User_School?.School_Id.ToString() ?? string.Empty),
                    new Claim("Student_Id", student.Student_Id.ToString())
                }),
                Expires = DateTime.UtcNow.AddDays(7),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            var tokenString = tokenHandler.WriteToken(token);

            return Ok(new { Token = "Bearer " + tokenString });

        }

        [HttpPost("reset/")]
        public async Task<ActionResult<User>> ResetPassword(string email)
        {
            var user = _context.Users.SingleOrDefault(u => u.User_email == email);
            if (user == null)
                return NotFound("Pas de compte asssocié à cet email.");
            string token = SendMail.GenerateResetToken();
            user.User_tokenReset = token;
            _context.SaveChanges();
            await _context.SaveChangesAsync();
            SendMail.SendForgetPasswordEmail(email,user.User_Id ,token);
            return Ok("Email envoyé");
        }


        [HttpPost("newpassword/")]
        public async Task<ActionResult<User>> ChangePassword(int user_id, string code, string password)
        {
            var user = _context.Users.SingleOrDefault(u => u.User_Id == user_id);
            if (user == null)
                return NotFound("Utilisateur non trouvé.");
            if (user.User_tokenReset != code)
                return BadRequest("Code invalide.");
            user.User_password = password;
            user.User_tokenReset = null;
            _context.SaveChanges();
            await _context.SaveChangesAsync();
            return Ok();
        }
    }
}
