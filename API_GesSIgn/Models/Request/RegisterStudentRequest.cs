using System.ComponentModel.DataAnnotations;

namespace API_GesSIgn.Models.Request
{
    public class RegisterStudentRequest
    {
        [Required]
        [MaxLength(255)]
        public string User_email { get; set; }

        [Required]
        public string User_password { get; set; }

        [Required]
        public string User_lastname { get; set; }

        [Required]
        public string User_firstname { get; set; }

        public string User_num { get; set; }

        public int? User_School_Id { get; set; }

        [Required]
        public int Student_Sector_Id { get; set; }
    }
}
