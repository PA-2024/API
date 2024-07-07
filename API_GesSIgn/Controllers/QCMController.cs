using API_GesSIgn.Models;
using API_GesSIgn.Models.Request;
using API_GesSIgn.Models.Response;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API_GesSIgn.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class QCMController : Controller
    {
        private readonly MonDbContext _context;

        public QCMController(MonDbContext context)
        {
            _context = context;
        }

        [HttpGet("qcm")]
        public async Task<ActionResult<IEnumerable<QCMDto>>> GetQcm()
        {
            var tmp =  await _context.QCMs
                .Include(q => q.QCM_SubjectHour)
                .ThenInclude(q => q.SubjectsHour_Subjects)
                .ToListAsync();
            List<QCMDto> qCMDtos = new List<QCMDto>();
            
            foreach (var qcm in tmp)
            {
                QCMDto add = new QCMDto();
                var question = await _context.Questions.Where(q => q.Question_QCM_Id == qcm.QCM_Id).ToListAsync();
                List<QuestionDto> questionDtos = new List<QuestionDto>();
                foreach (var q in question)
                {
                    var option = await _context.OptionQcm.Where(o => o.OptionQcm_Question_Id == q.Question_Id).ToListAsync();
                    questionDtos.Add(QuestionDto.FromQuestion(q, option));
                }
                add.Questions = questionDtos;
                add.Id = qcm.QCM_Id;
                add.Title = "QCM " + qcm.QCM_SubjectHour.SubjectsHour_Subjects.Subjects_Name;
                
                qCMDtos.Add(add);
            }

            return Ok(qCMDtos);


        }

        /// <summary>
        /// QCM par datez comme lié a une heure de cours
        /// </summary>
        /// <param name="dateRange"></param>
        /// <returns></returns>
        [HttpGet("qcmByRange")]
        public async Task<ActionResult<IEnumerable<QCMDto>>> GetQcmBYrange([FromQuery] DateRangeRequest dateRange)
        {
            var tmp = await _context.QCMs
                .Include(q => q.QCM_SubjectHour)
                .ThenInclude(q => q.SubjectsHour_Subjects)
                .Where(q => q.QCM_SubjectHour.SubjectsHour_DateStart >= dateRange.StartDate && q.QCM_SubjectHour.SubjectsHour_DateStart <= dateRange.EndDate)
                .ToListAsync();
            List<QCMDto> qCMDtos = new List<QCMDto>();

            foreach (var qcm in tmp)
            {
                qCMDtos.Add(await QcmToQcmDto(qcm));
            }

            return Ok(qCMDtos);

        }

        private async Task<QCMDto> QcmToQcmDto(QCM? qcm)
        {
            QCMDto add = new QCMDto();
            var question = await _context.Questions.Where(q => q.Question_QCM_Id == qcm.QCM_Id).ToListAsync();
            List<QuestionDto> questionDtos = new List<QuestionDto>();
            foreach (var q in question)
            {
                var option = await _context.OptionQcm.Where(o => o.OptionQcm_Question_Id == q.Question_Id).ToListAsync();
                questionDtos.Add(QuestionDto.FromQuestion(q, option));
            }
            add.Id = qcm.QCM_Id;
            add.Title = qcm.QCM_SubjectHour.SubjectsHour_Subjects.Subjects_Name;
            return add;
        }

        [HttpPost("DuplicateQcmByIdQcm/{id}/{SubjectsHour_id}")]
        public async Task<ActionResult<QCMDto>> DuplicateQcmById(int id, int SubjectsHour_id)
        {
            var qcm = await _context.QCMs.FirstOrDefaultAsync(q => q.QCM_Id == id);
            var subjectHour = await _context.SubjectsHour.FirstOrDefaultAsync(s => s.SubjectsHour_Id == SubjectsHour_id);
            if (qcm == null && subjectHour == null)
            {
                return NotFound();
            }

            QCMDto old = await QcmToQcmDto(qcm);
            var add = new QCM();
            add.QCM_SubjectHour_id = SubjectsHour_id;
            
            var newQcm = _context.QCMs.Add(add);

            foreach (var question in old.Questions)
            {
                Question qes = new Question();
                qes.Question_QCM_Id = newQcm.Entity.QCM_Id;
                qes.Question_Text = question.Text;
                var newQuestion = _context.Questions.Add(qes);
                foreach (var option in question.Options)
                {
                    OptionQcm opt = new OptionQcm();
                    opt.OptionQcm_Question_Id = newQuestion.Entity.Question_Id;
                    opt.OptionQcm_Text = option.Text;
                    _context.OptionQcm.Add(opt);
                }
            }
            await _context.SaveChangesAsync();

            return Ok("Dupplication efféctué");

        }

        




    }
}
