using API_GesSIgn.Models;
using API_GesSIgn.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API_GesSIgn.Controllers
{
    public class FIleController : Controller
    {
        private readonly CsvReaderService _csvReaderService;

        public FIleController(CsvReaderService csvReaderService)
        {
            _csvReaderService = csvReaderService;
        }

        [HttpPost("import")]
        [RoleRequirement("Gestion Ecole")]
        public async Task<IActionResult> AddUser([FromQuery] string file)
        {
            int id_school = int.Parse(User.FindFirst("SchoolId")?.Value);

            var (isSuccess, message) = await _csvReaderService.ImportUsersFromCsvAsync(file, id_school);

            Console.Write(file);

            if (!isSuccess)
            {
                return BadRequest(message);
            }

            return Ok(new { Status = message });
        }

        // GET: api/Classe/http://localhost:5000/api/File/User/1
        [HttpGet("Classe/{file}")]
        public IActionResult AddSectors(string file)
        {
            return Ok(new { Status = "Fichier Importé" });
        }

        // GET: api/QCM/http://localhost:5000/api/File/User/1
        [HttpGet("QCM/{file}")]
        public IActionResult AddQCM(string file)
        {
            return Ok(new { Status = "Fichier Importé" });
        }

    }
}
