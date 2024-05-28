using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace API_GesSIgn.Models
{
    /// <summary>
    /// Classe pour les utilisateurs
    /// </summary>
    public class User
    {
        [Key]
        public int User_Id { get; set; }

        [Required]
        public string User_email { get; set; }

        [Required]
        public string User_password { get; set; }

        [Required]
        public Roles User_Role { get; set; }

        public int? User_School_Id { get; set; }

        [ForeignKey("User_School_Id")]
        public School User_School { get; set; }

        [Required]
        public string User_lastname { get; set; }
        
        [Required]
        public string User_firstname { get; set; }

        public string User_num { get; set; }
    }
}
