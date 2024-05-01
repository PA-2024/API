using System.ComponentModel.DataAnnotations;

namespace API_GesSIgn.Models
{
    /// <summary>
    /// Permet une historisation des defférentes erreur soulevé dans l'api 
    /// </summary>
    public class Error 
    {
        [Key]
        public int Error_id {get; set;}

        public string? Error_Funtion {get; set;}

        [Required]
        public DateTime Error_DateTime {get; set;}
    }
}