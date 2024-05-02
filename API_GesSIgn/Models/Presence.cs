using System.ComponentModel.DataAnnotations;

namespace API_GesSIgn.Models
{
    /// <summary>
    /// Model de prescence de l'eleve   
    /// </summary>
    public class Presence
    {
        [Key]
        public int Presence_Id { get; set; }

        [Required]
        public User Presence_User { get; set; }

        [Required]
        public SubjectsHour Presence_SubjectsHour { get; set; }


        public Guid Prescence_Guid { get; set; }

    }
}
