using Microsoft.AspNetCore.Mvc;

namespace API_GesSIgn.Controllers
{
    public class QcmResult : Controller
    {

        private readonly MonDbContext _context;


        public QcmResult(MonDbContext context)
        {
            _context = context;
        }


        
    }
}
