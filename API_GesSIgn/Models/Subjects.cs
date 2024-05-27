using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace API_GesSIgn.Models
{
    /// <summary>
    /// Classe des matières
    /// </summary>
    public class Subjects
    {
        [Key]
        public int Subjects_Id { get; set; }

        [Required]
        public int Subjects_User_Id { get; set; }

        [ForeignKey("Subjects_User_Id")]
        public User Subjects_User { get; set; }

        [Required]
        public int Subjects_Sector_Id { get; set; }

        [ForeignKey("Subjects_Sector_Id")]
        public Sectors Subjects_Sectors { get; set; }
    }
}
