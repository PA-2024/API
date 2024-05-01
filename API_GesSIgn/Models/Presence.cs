using System.ComponentModel.DataAnnotations;

namespace API_GesSIgn.Models
{
    /// <summary>
    /// différent batiment de l'école  
    /// </summary>
    public class Presence
    {
        [Key]
        public int Presence_Id { get; set; }

        [Required]
        public User Presence_User { get; set; }

        [Required]
        public User Presence_SubjectsHour { get; set; }

    }
}
