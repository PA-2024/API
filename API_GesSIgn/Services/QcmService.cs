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
    }

    public class QcmService : IQcmService
    {
        private readonly MonDbContext _context;

        public QcmService(MonDbContext context)
        {
            _context = context;
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
    }
}
