using API_GesSIgn.Migrations;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace API_GesSIgn.Models
{
    public class AnswerQCM
    {
        [Key]
        public int AnswerQCM_Id { get; set; }

        public int AnswerQCM_QCM_Id { get; set; }

        [ForeignKey("AnswerQCM_QCM_Id")]
        public QCM AnswerQCM_QCM { get; set; }

        public int AnswerQCM_Question_Id { get; set; }

        [ForeignKey("AnswerQCM_Question_Id")]
        public Question AnswerQCM_Question { get; set; }

        /// <summary>
        /// listes des réponses possibles sous forme de string (séparées par des virgules)
        /// </summary>
        public string AnswerQCM_Answer { get; set; }


        public int AnswerQCM_Student_id { get; set; }

        [ForeignKey("AnswerQCM_Student_id")]
        public Student AnswerQCM_Student { get; set; }
    
    }
}
