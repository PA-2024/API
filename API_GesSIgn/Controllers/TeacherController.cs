using API_GesSIgn.Models;
using API_GesSIgn.Models.Request;
using Microsoft.AspNetCore.Mvc;

namespace Controllers {
    
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

        /*[HttpGet]
        public async Task<ActionResult<IEnumerable<User>>> GetTeachers()
        {
            return await _context.Users.ToListAsync();
        }
        $*/
    }
}