using System.ComponentModel.DataAnnotations;

namespace API_GesSIgn.Models
{
    /// <summary>
    /// Classe des matières 
    /// </summary>
    public class Subjects
    {
        [Key]
        public required int Subjects_Id { get; set; }

        /// <summary>
        /// Professeur de la matière
        /// </summary>
        [Required]
        public  User Subjects_User { get; set; }


        [Required]
        public required Sectors Subjects_Sectors { get; set; }  
      
    }
}
