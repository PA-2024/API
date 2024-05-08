using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using API_GesSIgn.Models;
using System.Threading.Tasks;
using System.Linq;

namespace API_GesSIgn.Controllers
{
    public class SectorsController : Controller
    {

        private readonly MonDbContext _context;

        public SectorsController(MonDbContext context)
        {
            _context = context;
        }

        // GET: Sectors
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            return View(await _context.Sectors.ToListAsync());
        }

        // GET: Sectors/Details/5
        [HttpGet("{id}")]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var sector = await _context.Sectors
                .FirstOrDefaultAsync(m => m.Sectors_Id == id);
            if (sector == null)
            {
                return NotFound();
            }

            return View(sector);
        }


    }
}
