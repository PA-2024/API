using System.ComponentModel.DataAnnotations;

namespace API_GesSIgn.Models
{
    /// <summary>
    /// Classe des matières 
    /// </summary>
    public class SubjectsHour
    {
        [Key]
        public int SubjectsHour_Id { get; set; }

        [Required]
        public  Sectors SubjectsHour_Sectors { get; set; }  

        public string? SubjectsHour_Rooom { get; set; }

        [Required]
        public DateTime SubjectsHour_Date { get; set; } 
      
    }
}