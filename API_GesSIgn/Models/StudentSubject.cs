using System.ComponentModel.DataAnnotations.Schema;

namespace API_GesSIgn.Models
{
    public class StudentSubject
    {
        public int StudentSubject_StudentId { get; set; }

        [ForeignKey("StudentSubject_StudentId")]
        public Student StudentSubject_Student { get; set; }

        public int StudentSubject_SubjectId { get; set; }

        [ForeignKey("StudentSubject_SubjectId")]
        public Subjects StudentSubject_Subject { get; set; }
    }
}
