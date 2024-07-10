using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace API_GesSIgn.Models
{
    /// <summary>
    /// Mod�le de pr�sence de l'�l�ve
    /// </summary>
    public class Presence
    {
        [Key]
        public int Presence_Id { get; set; }

        [Required]
        public int Presence_Student_Id { get; set; }


        public DateTime Presence_ScanDate { get; set; }

        public string? Presence_ScanInfo { get; set; }

        /// <summary>
        /// Eleve
        /// </summary>
        [ForeignKey("Presence_Student_Id")]
        public Student Presence_Student { get; set; }

        /// <summary>
        /// Heure de cours 
        /// </summary>
        [Required]
        public int Presence_SubjectsHour_Id { get; set; }

        [ForeignKey("Presence_SubjectsHour_Id")]
        public SubjectsHour Presence_SubjectsHour { get; set; }

        public int? Presence_ProofAbsence_Id { get; set; }

        [ForeignKey("Presence_ProofAbsence_Id")]
        public ProofAbsence? Presence_ProofAbsence { get; set; }

        
        [DefaultValue(false)]
        public bool Presence_Is { get; set; }
    }
}
