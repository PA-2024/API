using System.ComponentModel.DataAnnotations;

namespace API_GesSIgn.Models
{
    /// <summary>
    /// Classe des étudients 
    /// </summary>
    public class Sectors
    {
        /// <summary>
        /// id de la classe
        /// </summary>
        [Key]
        public int Sectors_Id { get; set; }

        /// <summary>
        /// Nom de classe
        /// </summary>
        [Required]
        public string Sectors_Name { get; set; }

    }
}
