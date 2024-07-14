using NuGet.Protocol.Plugins;
using System.ComponentModel.DataAnnotations;

namespace API_GesSIgn.Models.Response
{
    /// <summary>
    /// Détails des heures de matières 
    /// </summary>
    public class SubjectsHourDetailsDto
    {
        public int SubjectsHour_Id { get; set; }
        public DateTime SubjectsHour_DateStart { get; set; }
        public DateTime SubjectsHour_DateEnd { get; set; }
        public string? SubjectsHour_Room { get; set; }
        public string? SubjectsHour_TeacherComment { get; set; }
        public required BuildingDto Building { get; set; }

        public int? Presence_id { get; set; }

        public SubjectsdDto Subject { get; set; }

        public static SubjectsHourDetailsDto FromSubjectsHour(SubjectsHour subjectsHour, int? Presence_id = null)
        {
            return new SubjectsHourDetailsDto
            {
                SubjectsHour_Id = subjectsHour.SubjectsHour_Id,
                SubjectsHour_DateStart = subjectsHour.SubjectsHour_DateStart,
                SubjectsHour_DateEnd = subjectsHour.SubjectsHour_DateEnd,
                SubjectsHour_Room = subjectsHour.SubjectsHour_Room,
                SubjectsHour_TeacherComment = subjectsHour.SubjectsHour_TeacherComment,
                Building = BuildingDto.FromBuilding(subjectsHour.SubjectsHour_Bulding),
                Subject = SubjectsdDto.FromSubjects(subjectsHour.SubjectsHour_Subjects),
                Presence_id = Presence_id
            };
        }   
    }

    public class SubjectsHourDetailsDtoSUserProof
    {
        public int SubjectsHour_Id { get; set; }
        public DateTime SubjectsHour_DateStart { get; set; }
        public DateTime SubjectsHour_DateEnd { get; set; }
        public string? SubjectsHour_Room { get; set; }
        public string? SubjectsHour_TeacherComment { get; set; }
        public required BuildingDto Building { get; set; }

        public ProofAbsenceResponse? ProofAbsence { get; set; }

        public int? Presence_id { get; set; }

        public SubjectsdDto Subject { get; set; }

        public static SubjectsHourDetailsDtoSUserProof FromSubjectsHour(SubjectsHour subjectsHour, int Presence_id, ProofAbsence tmp)
        {
            return new SubjectsHourDetailsDtoSUserProof
            {
                SubjectsHour_Id = subjectsHour.SubjectsHour_Id,
                SubjectsHour_DateStart = subjectsHour.SubjectsHour_DateStart,
                SubjectsHour_DateEnd = subjectsHour.SubjectsHour_DateEnd,
                SubjectsHour_Room = subjectsHour.SubjectsHour_Room,
                SubjectsHour_TeacherComment = subjectsHour.SubjectsHour_TeacherComment,
                Building = BuildingDto.FromBuilding(subjectsHour.SubjectsHour_Bulding),
                Subject = SubjectsdDto.FromSubjects(subjectsHour.SubjectsHour_Subjects),
                Presence_id = Presence_id,
                ProofAbsence = ProofAbsenceResponse.FromProofAbsence(tmp)
            };
        }
    }

    public class SubjectsHourSimplify
    {
        public int SubjectsHour_Id { get; set; }
        public DateTime SubjectsHour_DateStart { get; set; }
        public DateTime SubjectsHour_DateEnd { get; set; }
        public string? SubjectsHour_Room { get; set; }
        public string? SubjectsHour_TeacherComment { get; set; }
        public BuildingDto Building { get; set; }
        public SubjectsdDto SubjectsHour_Subject { get; set; }

        public static SubjectsHourSimplify FromSubjectsHour(SubjectsHour subjectsHour)
        {
            return new SubjectsHourSimplify
            {
                SubjectsHour_Id = subjectsHour.SubjectsHour_Id,
                SubjectsHour_DateStart = subjectsHour.SubjectsHour_DateStart,
                SubjectsHour_DateEnd = subjectsHour.SubjectsHour_DateEnd,
                SubjectsHour_Room = subjectsHour.SubjectsHour_Room,
                SubjectsHour_TeacherComment = subjectsHour.SubjectsHour_TeacherComment,
                SubjectsHour_Subject = SubjectsdDto.FromSubjects(subjectsHour.SubjectsHour_Subjects),
                Building = BuildingDto.FromBuilding(subjectsHour.SubjectsHour_Bulding),
            };
        }   
    }

    /// <summary>
    /// SubjectsHour avec l'information d'un élèves sur leur presence
    /// </summary>
    public class SubjectsHourSimplyWithPrescense
    {
        public int SubjectsHour_Id { get; set; }

        public DateTime SubjectsHour_DateStart { get; set; } 

        public DateTime SubjectsHour_DateEnd { get; set; }  

        public required SubjectsdDto SubjectsHour_Subject { get; set; }

        /// <summary>
        ///
        /// </summary>
        public ProofAbsenceResponse? proofAbsence { get; set; }

        public bool StudentIsPresent { get; set; }

        public static SubjectsHourSimplyWithPrescense FromSubjectsHour(SubjectsHour subjectsHour, bool isPresent)
        {
            return new SubjectsHourSimplyWithPrescense
            {
                SubjectsHour_Id = subjectsHour.SubjectsHour_Id,
                SubjectsHour_DateStart = subjectsHour.SubjectsHour_DateStart,
                SubjectsHour_DateEnd = subjectsHour.SubjectsHour_DateEnd,
                SubjectsHour_Subject = SubjectsdDto.FromSubjects(subjectsHour.SubjectsHour_Subjects),
                StudentIsPresent = isPresent
            };
        }   
    }


    

    /// <summary>
    /// SubjectsHour avec l'information des élèves sur leur presence
    /// </summary>
    public class SubjectsHourDetailsWithStudentsDto
    {
        public int SubjectsHour_Id { get; set; }

        public DateTime SubjectsHour_DateStart { get; set; }
        public DateTime SubjectsHour_DateEnd { get; set; }
        public string SubjectsHour_Room { get; set; }
        public BuildingDto Building { get; set; }
        public SubjectsdDto Subject { get; set; }
        public List<StudentIsPresent> Students { get; set; }
    }


}
