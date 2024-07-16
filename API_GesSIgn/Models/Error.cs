using System.ComponentModel;
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

        public string? Error_Description { get; set; }

        [Required]
        [DefaultValue(false)]
        public bool Error_Solved { get; set; }

        public Error(string Error_Funtion, DateTime Error_DateTime, string Error_Description, bool Error_Solved)
        {
            this.Error_Funtion = Error_Funtion;
            this.Error_DateTime = Error_DateTime;
            this.Error_Description = Error_Description;
            this.Error_Solved = Error_Solved;
        }

    }
}