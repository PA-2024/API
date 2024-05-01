using System.ComponentModel.DataAnnotations;

namespace API_GesSIgn.Models
{
    /// <summary>
    /// Class pour les utilisateurs S
    /// </summary>
    public class User
    {
        [Key]
        public  int User_Id { get; set; }

        [Required]
        public string User_email { get; set; }

        [Required]
        public string User_password { get; set; }
    }
}
