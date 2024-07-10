using API_GesSIgn.Models.Response;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API_GesSIgn.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class QcmResult : Controller
    {

        private readonly MonDbContext _context;


        public QcmResult(MonDbContext context)
        {
            _context = context;
        }

        public async Task<ActionResult<IEnumerable<QcmResultResponce>>> AllResultsQcmForOneQcm(int id)
        {
            var qcm = await _context.QcmResult
                .Include(q => q.QcmResult_Student)
                .Where(m => m.QcmResult_QCM_Id == id).ToListAsync();

            List<QcmResultResponce> result = new List<QcmResultResponce>();

            foreach (var q in qcm)
            {
                var qcmResult = new QcmResultResponce
                {
                    QcmResult_Student = StudentSimplifyDto.FromStudent(q.QcmResult_Student),
                    QcmResult_Score = q.QcmResult_Score,
                    QcmResult_Id = q.QcmResult_Id
                };

                result.Add(qcmResult);
            }

            return Ok(qcm);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="id">id du qcm</param>
        /// <returns></returns>
        public async Task<ActionResult<IEnumerable<QcmResultDetails>>> AllResultsQcmForOneQcmDetails(int id)
        {
            var qcm = await _context.AnswerQCM
                .Include(q => q.AnswerQCM_Student)
                .Where(m => m.AnswerQCM_QCM_Id == id).ToListAsync();

            List<QcmResultDetails> result = new List<QcmResultDetails>();

            foreach (var q in qcm)
            {
                var qcmResult = new QcmResultDetails
                {
                    QcmResult_Student = StudentSimplifyDto.FromStudent(q.AnswerQCM_Student),
                    QcmResultDetails_Answer = q.AnswerQCM_Question,
                    QcmResultDetails_Question_id = q.AnswerQCM_Question_Id
                };

                result.Add(qcmResult);
            }

            return Ok(qcm);
        }

    }
}
