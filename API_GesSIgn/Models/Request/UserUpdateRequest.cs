using System.ComponentModel.DataAnnotations;

namespace API_GesSIgn.Models
{
    public class UserUpdateRequest
    {
        [MaxLength(255)]
        public string? User_email { get; set; }

        public string? User_lastname { get; set; }

        public string? User_firstname { get; set; }

        public string? User_num { get; set; }
    }

    public class UserRequest
    {
        [MaxLength(255)]
        public required string? User_email { get; set; }
    
        public required string User_lastname { get; set; }
    
        public required string User_firstname { get; set; }
    
        public string? User_num { get; set; }
    
        public int user_school_id { get; set; }
    }
}
