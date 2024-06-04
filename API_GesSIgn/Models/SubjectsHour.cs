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

        /// <summary>
        /// ID de la matière
        /// </summary>
        [Required]
        public int SubjectsHour_Subjects_Id { get; set; }

        [ForeignKey("SubjectsHour_Sector_Id")]
        public Subjects? SubjectsHour_Subjects { get; set; }


        public int? SubjectsHour_Bulding_Id { get; set; }


        [ForeignKey("SubjectsHour_Bulding_Id")]
        public Building? SubjectsHour_Bulding { get; set; }

        public string? SubjectsHour_Room { get; set; }

        [Required]
        public DateTime SubjectsHour_DateStart { get; set; }

        [Required]
        public DateTime SubjectsHour_DateEnd { get; set; }
    }
}
