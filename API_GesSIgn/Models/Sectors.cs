using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace API_GesSIgn.Models
{
    /// <summary>
    /// Classe des secteurs (classes d'étudiants)
    /// </summary>
    public class Sectors
    {
        /// <summary>
        /// ID du secteur
        /// </summary>
        [Key]
        public int Sectors_Id { get; set; }

        /// <summary>
        /// Nom du secteur
        /// </summary>
        [Required]
        public string Sectors_Name { get; set; }

        [Required]
        public int Sectors_School_Id { get; set; }

        [ForeignKey("Sectors_School_Id")]
        public School Sectors_School { get; set; }
    }
}
