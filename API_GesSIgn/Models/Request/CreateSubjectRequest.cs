using System.ComponentModel.DataAnnotations;

namespace API_GesSIgn.Models.Request
{
    public class CreateSubjectRequest
    {
        public int Subjects_Id { get; set; }

        [Required]
        public string Subjects_Name { get; set; }

        [Required]
        public int Subjects_User_Id { get; set; }
    }
}
