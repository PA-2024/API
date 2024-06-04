using System.ComponentModel.DataAnnotations;

namespace API_GesSIgn.Models.Request
{
    public class CreateSubjectHourRequest
    {
        public int SubjectsHour_Subjects_Id { get; set; }

        public int SubjectsHour_Building_Id { get; set; }

        public string? SubjectsHour_Room { get; set; }

        [Required]
        public DateTime SubjectsHour_DateStart { get; set; }

        [Required]
        public DateTime SubjectsHour_DateEnd { get; set; }
    }
}
