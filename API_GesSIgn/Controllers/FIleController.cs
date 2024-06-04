using API_GesSIgn.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API_GesSIgn.Controllers
{
    public class FIleController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        // GET: api/User/http://localhost:5000/api/File/User/1
        [HttpGet("User/{file}")]
        public IActionResult AddUser(string file)
        {
            return Ok(new { Status = "Fichier Importé" });
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
