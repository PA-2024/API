using System.ComponentModel.DataAnnotations;

namespace API_GesSIgn.Models
{
    /// <summary>
    /// Class lié au QCM
    /// </summary>
    public class QCM 
    {
        [Key]
        public int QCM_Id { get; set;}

        [Required]
        public User QCM_Teacher {get; set;}

        [Required]
        public bool QCM_Done {get; set;}
    }
}