using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace API_GesSIgn.Models
{
    /// <summary>
    /// Modèle de présence de l'élève
    /// </summary>
    public class Presence
    {
        [Key]
        public int Presence_Id { get; set; }

        [Required]
        public int Presence_User_Id { get; set; }

        [ForeignKey("Presence_User_Id")]
        public User Presence_User { get; set; }

        [Required]
        public int Presence_SubjectsHour_Id { get; set; }

        [ForeignKey("Presence_SubjectsHour_Id")]
        public SubjectsHour Presence_SubjectsHour { get; set; }
        
        [DefaultValue(false)]
        public bool Presence_Is { get; set; }
    }
}
