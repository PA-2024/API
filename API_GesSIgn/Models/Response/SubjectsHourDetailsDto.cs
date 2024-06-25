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
        public string SubjectsHour_Room { get; set; }
        public string? SubjectsHour_TeacherComment { get; set; }
        public BuildingDto Building { get; set; }
        public SubjectDetailsWithOutStudentSimplifyDto Subject { get; set; }

        public static SubjectsHourDetailsDto FromSubjectsHour(SubjectsHour subjectsHour)
        {
            return new SubjectsHourDetailsDto
            {
                SubjectsHour_Id = subjectsHour.SubjectsHour_Id,
                SubjectsHour_DateStart = subjectsHour.SubjectsHour_DateStart,
                SubjectsHour_DateEnd = subjectsHour.SubjectsHour_DateEnd,
                SubjectsHour_Room = subjectsHour.SubjectsHour_Room,
                SubjectsHour_TeacherComment = subjectsHour.SubjectsHour_TeacherComment,
                Building = BuildingDto.FromBuilding(subjectsHour.SubjectsHour_Bulding),
                Subject = new SubjectDetailsWithOutStudentSimplifyDto
                {
                    Subjects_Id = subjectsHour.SubjectsHour_Subjects.Subjects_Id,
                    Subjects_Name = subjectsHour.SubjectsHour_Subjects.Subjects_Name,
                    Teacher = UserSimplifyDto.FromUser(subjectsHour.SubjectsHour_Subjects.Subjects_User)
                }
            };
        }   
    }


    public class SubjectsHourSimplify
    {
        public int SubjectsHour_Id { get; set; }
        public DateTime SubjectsHour_DateStart { get; set; }
        public DateTime SubjectsHour_DateEnd { get; set; }
        public string SubjectsHour_Room { get; set; }
        public string? SubjectsHour_TeacherComment { get; set; }
        public BuildingDto Building { get; set; }

        public static SubjectsHourDetailsDto FromSubjectsHour(SubjectsHour subjectsHour)
        {
            return new SubjectsHourDetailsDto
            {
                SubjectsHour_Id = subjectsHour.SubjectsHour_Id,
                SubjectsHour_DateStart = subjectsHour.SubjectsHour_DateStart,
                SubjectsHour_DateEnd = subjectsHour.SubjectsHour_DateEnd,
                SubjectsHour_Room = subjectsHour.SubjectsHour_Room,
                SubjectsHour_TeacherComment = subjectsHour.SubjectsHour_TeacherComment,
                Building = BuildingDto.FromBuilding(subjectsHour.SubjectsHour_Bulding),
            };
        }   
    }
}
