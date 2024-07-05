using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace API_GesSIgn.Models
{
    public class Question
    {
        [Key]
        public int Question_Id { get; set; }

        public string Question_Text { get; set; }

        public int Question_QCM_Id { get; set; }

        [ForeignKey("Question_QCM_Id")]
        public QCM? Question_QCM { get; set; }
        
    }

    public class OptionQcm
    {
        [Key]
        public int OptionQcm_id { get; set; }

        public string OptionQcm_Text { get; set; }

        public bool OptionQcm_IsCorrect { get; set; }

        public int OptionQcm_Question_Id { get; set; }

        [ForeignKey("OptionQcm_Question_Id")]
        public Question? OptionQcm_Question { get; set; }
    }
}
