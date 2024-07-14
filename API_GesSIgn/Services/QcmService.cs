using API_GesSIgn.Models;
using API_GesSIgn.Models.Response;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace API_GesSIgn.Services
{
    public interface IQcmService
    {
        Task<QCMDto> GetQCMByIdAsync(int id);
        Task<Student> GetStudentByIdAsync(int id);
        Task<QCM> GetQCMById(int id);
        Task AddAnswer(HashSet<int> answer, int student_id, int idqcm, int idquestion);

        Task SauvScore(List<StudentQcm> students, int idqcm);
    }

    public class QcmService : IQcmService
    {
        private readonly MonDbContext _context;

        public QcmService(MonDbContext context)
        {
            _context = context;
        }

        public async Task<QCM> GetQCMById(int id)
        {
            var qcm = await _context.QCMs.FirstOrDefaultAsync(q => q.QCM_Id == id);
            return qcm;
        }

            public async Task<QCMDto> GetQCMByIdAsync(int id)
        {
            var qcm = await _context.QCMs.FirstOrDefaultAsync(q => q.QCM_Id == id);


            if (qcm == null)
            {
                return null;
            }

            var question = await _context.Questions.Where(q => q.Question_QCM_Id == qcm.QCM_Id).ToListAsync();

            List<QuestionDto> questionDtos = new List<QuestionDto>();

            foreach (var q in question)
            {
                var option = await _context.OptionQcm.Where(o => o.OptionQcm_Question_Id == q.Question_Id).ToListAsync();
                questionDtos.Add(QuestionDto.FromQuestion(q, option));
            }


            var qcmDto = new QCMDto
            {
                Id = qcm.QCM_Id,
                Title = "QCM Title", // TODO: Remplacez par le titre réel
                Questions = questionDtos,
            };

            return qcmDto;
        }

        public async Task<Student> GetStudentByIdAsync(int id)
        {
            return await _context.Students.FirstOrDefaultAsync(s => s.Student_Id == id);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="answer"></param>
        /// <param name="student"></param>
        /// <param name="idqcm"></param>
        /// <param name="idquestion"></param>
        public async Task AddAnswer(HashSet<int> answer, int student_id, int idqcm, int idquestion)
        {
            try { 
                AnswerQCM answerQCM = new AnswerQCM();
                answerQCM.AnswerQCM_Answer = string.Join(",", answer);
                answerQCM.AnswerQCM_Student_id = student_id;
                answerQCM.AnswerQCM_QCM_Id = idqcm;
                answerQCM.AnswerQCM_Question_Id = idquestion;
                _context.AnswerQCM.Add(answerQCM);
                await _context.SaveChangesAsync();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }

        }

        public async Task SauvScore(List<StudentQcm> students, int idqcm)
        {
            try
            {
                List<QcmResult> qcmResults = new List<QcmResult>();

                foreach (StudentQcm student in students)
                {
                    QcmResult qcmResult = new QcmResult();
                    qcmResult.QcmResult_Student_Id = Convert.ToInt32(student.Student_Id);
                    qcmResult.QcmResult_QCM_Id = idqcm;
                    qcmResult.QcmResult_Score = student.Score;
                    qcmResults.Add(qcmResult);
                }

                _context.QcmResult.AddRange(qcmResults);
                await _context.SaveChangesAsync();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }

        }
    }
}
