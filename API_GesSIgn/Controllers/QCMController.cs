using API_GesSIgn.Helpers;
using API_GesSIgn.Models;
using API_GesSIgn.Models.Request;
using API_GesSIgn.Models.Response;
using Azure.Core;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using static System.Collections.Specialized.BitVector32;

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

        /// <summary>
        /// Get qcm 
        /// </summary>
        /// <param name="pageNumber">page a rechercher</param>
        /// <param name="pageSize">Nb de résultat par page</param>
        /// <returns></returns>
        [HttpGet("qcm")]
        [RoleRequirement("Gestion Ecole")]
        public async Task<ActionResult<IEnumerable<QCMDto>>> GetQcm(int pageNumber = 1, int pageSize = 10)
        {
            if (pageNumber <= 0 || pageSize <= 0)
            {
                return BadRequest("Page number and page size must be greater than zero.");
            }

            var totalQcmCount = await _context.QCMs.CountAsync();
            var totalPages = (int)Math.Ceiling(totalQcmCount / (double)pageSize);

            if (pageNumber > totalPages)
            {
                return BadRequest($"Page number exceeds total pages ({totalPages}).");
            }

            var schoolIdClaim = User.FindFirst("SchoolId")?.Value;
            if (string.IsNullOrEmpty(schoolIdClaim))
            {
                return BadRequest("School ID not found in token.");
            }

            var qcmList = await _context.QCMs
                .Include(q => q.QCM_SubjectHour)
                .ThenInclude(q => q.SubjectsHour_Subjects)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .Where(string.IsNullOrEmpty(schoolIdClaim) ? (q => q.QCM_SubjectHour.SubjectsHour_Subjects.Subjects_School_Id == int.Parse(schoolIdClaim)) : (q => true))
                .ToListAsync();

            List<QCMDto> qcmDtos = new List<QCMDto>();

            foreach (var qcm in qcmList)
            {
                QCMDto add = QcmToQcmDto(qcm).Result;
                qcmDtos.Add(add);
            }

            var paginatedResult = new PaginatedResult<QCMDto>
            {
                Items = qcmDtos,
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalPages = totalPages,
                TotalItems = totalQcmCount
            };

            return Ok(paginatedResult);
        }


        /// <summary>
        /// Pour les professeurs, récupérer les QCMs par date
        /// </summary>
        /// <param name="pageNumber"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        [HttpGet("qcmforTeacher")]
        [RoleRequirement("Professeur")]
        public async Task<ActionResult<IEnumerable<QCMDto>>> GetQcmForTeacher(int pageNumber = 1, int pageSize = 10)
        {
            if (pageNumber <= 0 || pageSize <= 0)
            {
                return BadRequest("Page number and page size must be greater than zero.");
            }

            var totalQcmCount = await _context.QCMs.CountAsync();
            var totalPages = (int)Math.Ceiling(totalQcmCount / (double)pageSize);

            if (pageNumber > totalPages)
            {
                return BadRequest($"Page number exceeds total pages ({totalPages}).");
            }

            var schoolIdClaim = User.FindFirst("SchoolId")?.Value;
            var userIdClaim = User.FindFirst("UserId")?.Value;
            if (string.IsNullOrEmpty(schoolIdClaim))
            {
                return BadRequest("School ID not found in token.");
            }

            var qcmList = await _context.QCMs
                .Include(q => q.QCM_SubjectHour)
                .ThenInclude(q => q.SubjectsHour_Subjects)
                .ThenInclude(q => q.Subjects_User)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .Where(q => q.QCM_SubjectHour.SubjectsHour_Subjects.Subjects_School_Id == int.Parse(schoolIdClaim)
                && q.QCM_SubjectHour.SubjectsHour_Subjects.Subjects_User.User_Id == Convert.ToInt32(userIdClaim) )
                .ToListAsync();

            List<QCMDto> qcmDtos = new List<QCMDto>();

            foreach (var qcm in qcmList)
            {
                QCMDto add = QcmToQcmDto(qcm).Result;
                qcmDtos.Add(add);
            }

            var paginatedResult = new PaginatedResult<QCMDto>
            {
                Items = qcmDtos,
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalPages = totalPages,
                TotalItems = totalQcmCount
            };

            return Ok(paginatedResult);
        }

        /// <summary>
        /// QCM par datez comme lié a une heure de cours, sans les réponses
        /// </summary>
        /// <param name="dateRange"></param>
        /// <returns></returns>
        [HttpGet("qcmByRange")]
        public async Task<ActionResult<IEnumerable<QCMDto>>> GetQcmByrange([FromQuery] DateRangeRequest dateRange)
        {
            var tmp = await _context.QCMs
                .Include(q => q.QCM_SubjectHour)
                .ThenInclude(q => q.SubjectsHour_Subjects)
                .Where(q => q.QCM_SubjectHour.SubjectsHour_DateStart >= dateRange.StartDate && q.QCM_SubjectHour.SubjectsHour_DateStart <= dateRange.EndDate)
                .ToListAsync();

            
            // TODO a fix

            List<QCMDto> qCMDtos = new List<QCMDto>();

            foreach (var qcm in tmp)
            {
                QCMDto add = new QCMDto();               
                add.Id = qcm.QCM_Id;
                add.Title = "QCM " + qcm.QCM_SubjectHour.SubjectsHour_Subjects.Subjects_Name;

                qCMDtos.Add(add);
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
            add.Questions = questionDtos;
            add.Id = qcm.QCM_Id;
            add.Title = "QCM " + qcm.QCM_SubjectHour.SubjectsHour_Subjects.Subjects_Name;
            return add;
        }

        [HttpPost("DuplicateQcmByIdQcm/{id}/{SubjectsHour_id}")]
        [RoleRequirement(["Professeur", "Gestion Ecole"])]
        public async Task<ActionResult<QCMDto>> DuplicateQcmById(int id, int SubjectsHour_id)
        {
            var qcm = await _context.QCMs
                .Include(q => q.QCM_SubjectHour)
                .ThenInclude(q => q.SubjectsHour_Subjects)
                .FirstOrDefaultAsync(q => q.QCM_Id == id);
            var subjectHour = await _context.SubjectsHour.FirstOrDefaultAsync(s => s.SubjectsHour_Id == SubjectsHour_id);
            if (qcm == null && subjectHour == null)
            {
                return NotFound();
            }

            QCMDto old = await QcmToQcmDto(qcm);
            var add = new QCM();
            add.QCM_SubjectHour_id = SubjectsHour_id;
            
            var newQcm = _context.QCMs.Add(add);
            await _context.SaveChangesAsync();
            

            foreach (var question in old.Questions)
            {
                Question qes = new Question();
                qes.Question_QCM_Id = newQcm.Entity.QCM_Id;
                qes.Question_Text = question.Text;
                var newQuestion = _context.Questions.Add(qes);
                await _context.SaveChangesAsync();
                foreach (var option in question.Options)
                {
                    OptionQcm opt = new OptionQcm();
                    opt.OptionQcm_Question_Id = newQuestion.Entity.Question_Id;
                    opt.OptionQcm_Text = option.Text;
                    _context.OptionQcm.Add(opt);
                    await _context.SaveChangesAsync();
                }
            }
            await _context.SaveChangesAsync();

            return Ok("Dupplication efféctué");

        }

        [HttpPost("AddQuestion/{QCM_id}")]
        [RoleRequirement(["Professeur", "Gestion Ecole"])]
        public async Task<ActionResult<QuestionDto>> AddQuestion(int QCM_id, CreateQuestionRequest request)
        {
            var qcm = await _context.QCMs.FirstOrDefaultAsync(q => q.QCM_Id == QCM_id);
            if (qcm == null)
            {
                return NotFound();
            }

            var question = new Question();
            question.Question_QCM_Id = QCM_id;
            question.Question_Text = request.Text;
            var newQuestion = _context.Questions.Add(question);
            await _context.SaveChangesAsync();
            foreach (var option in request.Options)
            {
                OptionQcm opt = new OptionQcm();
                opt.OptionQcm_Question_Id = newQuestion.Entity.Question_Id;
                opt.OptionQcm_Text = option.Text;
                opt.OptionQcm_IsCorrect = option.IsCorrect;
                _context.OptionQcm.Add(opt);
                await _context.SaveChangesAsync();
            }

            await _context.SaveChangesAsync();

            return Ok("Question ajouté");
        }

        [HttpPost("QCM")]
        [RoleRequirement(["Professeur", "Gestion Ecole"])]
        public async Task<ActionResult<QCMDto>> AddQcm(CreateQCMRequest createQCM)
        {
            QCM qcm = new QCM();

            var schoolIdClaim = User.FindFirst("SchoolId")?.Value;
            if (string.IsNullOrEmpty(schoolIdClaim))
            {
                return BadRequest("School ID not found in token.");
            }

            var subjectHour = await _context.SubjectsHour
                .Include(p => p.SubjectsHour_Subjects)
                .Where(p => p.SubjectsHour_Subjects.Subjects_School_Id == Convert.ToInt32(schoolIdClaim))
                .FirstOrDefaultAsync(s => s.SubjectsHour_Id == createQCM.SubjectHour_id);

            if (subjectHour == null)
            {
                return NotFound("SubjectHour not found");
            }
            qcm.QCM_SubjectHour = subjectHour;
            var newQcm = _context.Add(qcm);
            await _context.SaveChangesAsync();
            if (createQCM != null)
            {
                foreach (var q in createQCM.Questions) {
                    var question = new Question();
                    question.Question_QCM_Id = newQcm.Entity.QCM_Id;
                    question.Question_Text = q.Text;
                    var newQuestion = _context.Questions.Add(question);
                    await _context.SaveChangesAsync();
                    foreach (var option in q.Options)
                    {
                        OptionQcm opt = new OptionQcm();
                        opt.OptionQcm_Question_Id = newQuestion.Entity.Question_Id;
                        opt.OptionQcm_Text = option.Text;
                        opt.OptionQcm_IsCorrect = option.IsCorrect;
                        _context.OptionQcm.Add(opt);
                        await _context.SaveChangesAsync();
                    }
                }
            }
            return Ok("QCM ajouté");
        }

        




    }
}
