using System.ComponentModel.DataAnnotations;

namespace API_GesSIgn.Models
{
    public class LoginRequest
    {
        [Required]
        public string User_email { get; set; }

        [Required]
        public string User_password { get; set; }
    }
}
