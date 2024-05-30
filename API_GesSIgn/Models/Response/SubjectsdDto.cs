using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace API_GesSIgn.Models.Response
{
    public class SubjectsdDto
    {
        [Key]
        public int Subjects_Id { get; set; }

        [Required]
        public string Subjects_Name { get; set; }

        [Required]
        public UserSimplifyDto Teacher { get; set; }

    }
}
