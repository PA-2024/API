using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace API_GesSIgn.Models
{
    /// <summary>
    /// Classe des heures de matières
    /// </summary>
    public class SubjectsHour
    {
        [Key]
        public int SubjectsHour_Id { get; set; }

        [Required]
        public int SubjectsHour_Sector_Id { get; set; }

        [ForeignKey("SubjectsHour_Sector_Id")]
        public Sectors? SubjectsHour_Sectors { get; set; }

        public string? SubjectsHour_Room { get; set; }

        [Required]
        public DateTime SubjectsHour_DateStart { get; set; }

        [Required]
        public DateTime SubjectsHour_DateEnd { get; set; }
    }
}
