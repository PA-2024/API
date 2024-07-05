using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace API_GesSIgn.Models
{
    /// <summary>
    /// Class lié au QCM
    /// </summary>
    public class QCM 
    {
        /// <summary>
        /// Id du QCM
        /// </summary>
        [Key]
        public int QCM_Id { get; set;}


        /// <summary>
        /// Si true, impossible de relancer le QCM
        /// </summary>
        [Required]
        [DefaultValue(false)]
        public bool QCM_Done {get; set;}

        /// <summary>
        /// QCM lié à une heure de cours 
        /// </summary>
        [Required]
        public int QCM_SubjectHour_id { get; set; }

        [ForeignKey("QCM_SubjectHour_id")]
        public SubjectsHour QCM_SubjectHour { get; set; }

    }
}