using System.ComponentModel.DataAnnotations;

namespace API_GesSIgn.Models.Request
{
    public class StudentSubjectRequest
    {
        public int Student_Id { get; set; }
        public int Subject_Id { get; set; }
    }

    public class AddStudentsToSubjectRequest
    {
        [Required]
        public int Subject_Id { get; set; }

        [Required]
        public List<int> StudentIds { get; set; }
    }
}
