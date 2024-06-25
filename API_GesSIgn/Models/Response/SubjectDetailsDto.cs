using System.Collections.Generic;

namespace API_GesSIgn.Models.Response
{
    /// <summary>
    /// Matière détaillée avec les élèves inscrits
    /// </summary>
    public class SubjectDetailsDto
    {
        public int Subjects_Id { get; set; }
        public string Subjects_Name { get; set; }
        public UserSimplifyDto Teacher { get; set; }
        public List<StudentSimplifyDto>? Students { get; set; }
    }

    public class SubjectDetailsWithOutStudentSimplifyDto
    {
        public int Subjects_Id { get; set; }
        public string Subjects_Name { get; set; }
        public UserSimplifyDto Teacher { get; set; }
    }

    /// <summary>
    /// Élève simplifié 
    /// </summary>
    public class StudentSimplifyDto
    {
        public int Student_Id { get; set; }

        public required Sectors Student_Sectors { get; set; }

        public required UserSimplifyDto Student_User { get; set; }

        public static StudentSimplifyDto FromStudent(Student student)
        {
            return new StudentSimplifyDto
            {
                Student_Id = student.Student_Id,
                Student_Sectors = student.Student_Sectors,
                Student_User = UserSimplifyDto.FromUser(student.Student_User)
            };
        }
    }

    public class StudentIsPresent
    {
        public int Student_Id { get; set; }
        public UserSimplifyDto Student_User { get; set; }
        public bool IsPresent { get; set; }
    }

    public class SubjectsHourDetailsWithStudentsDto
    {
        public int SubjectsHour_Id { get; set; }
        public DateTime SubjectsHour_DateStart { get; set; }
        public DateTime SubjectsHour_DateEnd { get; set; }
        public string SubjectsHour_Room { get; set; }
        public BuildingDto Building { get; set; }
        public SubjectDetailsWithOutStudentSimplifyDto Subject { get; set; }
        public List<StudentIsPresent> Students { get; set; }
    }
}
