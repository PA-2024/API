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


    public class ProofAbsenceRequest
    {
        public int ProofAbsence_Id { get; set; }
        public required string ProofAbsence_SchoolComment { get; set; }

        public int ProofAbsence_Status { get; set; }

        public List<int>? Presence_id { get; set; }
    }

    public class CreateProofAbsenceRequest
    {
        public required string ProofAbsence_StudentComment { get; set; }

        public string ProofAbsence_UrlFile { get; set; }

    }
}
