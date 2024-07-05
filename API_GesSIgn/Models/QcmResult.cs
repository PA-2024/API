using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace API_GesSIgn.Models
{
    public class QcmResult
    {
        [Key]
        public int QcmResult_Id { get; set; }

        [Required]
        public int QcmResult_Student_Id { get; set; }

        [Required]
        [ForeignKey("QcmResult_Student_Id")]
        public Student QcmResult_Student { get; set; }

        [Required]
        public int QcmResult_QCM_Id { get; set; }

        [Required]
        [ForeignKey("QcmResult_QCM_Id")]
        public QCM QcmResult_QCM { get; set; }

        [Required]
        public int QcmResult_Score { get; set; }
    }
}
