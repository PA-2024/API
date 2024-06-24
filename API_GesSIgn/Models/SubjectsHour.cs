using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace API_GesSIgn.Models
{
    /// <summary>
    /// Classe des heures de cours 
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

        /// <summary>
        /// ID de la salle
        /// </summary>
        public string? SubjectsHour_Room { get; set; }

        /// <summary>
        /// Date de début du cours
        /// </summary>
        [Required]
        public DateTime SubjectsHour_DateStart { get; set; }

        /// <summary>
        /// Date de fin du cours
        /// </summary>
        [Required]
        public DateTime SubjectsHour_DateEnd { get; set; }

        /// <summary>
        /// Commentaire du professeur
        /// </summary>
        public string? SubjectsHour_TeacherComment { get; set; }
    }
}
