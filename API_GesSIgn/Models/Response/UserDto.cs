using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace API_GesSIgn.Models.Response
{
    public class UserDto
    {
    }

    public class UserSimplifyDto
    {
        public int User_Id { get; set; }

        [MaxLength(255)]
        [Required]
        public string User_email { get; set; }

        [Required]

        public string User_lastname { get; set; }
        [Required]
        public string User_firstname { get; set; }

        public string User_num { get; set; }
    }
}
