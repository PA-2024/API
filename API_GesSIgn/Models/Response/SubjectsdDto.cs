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

    public class SubjectsSimplify
    {
        [Key]
        public int Subjects_Id { get; set; }

        [Required]
        public string Subjects_Name { get; set; }

        public static SubjectsSimplify FromSubjects(Subjects subjects)
        {
            return new SubjectsSimplify
            {
                Subjects_Id = subjects.Subjects_Id,
                Subjects_Name = subjects.Subjects_Name
            };
        }

    }
}
