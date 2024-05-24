using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace API_GesSIgn.Models
{
    /// <summary>
    /// Class pour les utilisateurs S
    /// </summary>
    public class User : IdentityUser
    {
        [Key]
        public  int User_Id { get; set; }

        [Required]
        public string User_email { get; set; }

        [Required]
        public string User_password { get; set; }

        [Required]
        public Roles User_Role { get; set; }

        /// <summary>
        /// School de l'utilisateur
        /// </summary>
        public School? User_School { get; set; }
    }
}
