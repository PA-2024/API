﻿using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace API_GesSIgn.Models.Response
{
    /// <summary>
    /// Matière détaillée avec les élèves inscrits
    /// </summary>
    public class SubjectDetailsListStudent
    {
        public int Subjects_Id { get; set; }
        public string Subjects_Name { get; set; }
        public UserSimplifyDto Teacher { get; set; }
        public List<StudentSimplifyDto>? Students { get; set; }
    }

    public class SubjectsdDto
    {
        [Key]
        public int Subjects_Id { get; set; }

        [Required]
        public string Subjects_Name { get; set; }

        [Required]
        public UserSimplifyDto? Teacher { get; set; }

        public static SubjectsdDto FromSubjects(Subjects subjects)
        {
            SubjectsdDto res = new SubjectsdDto();
            res.Subjects_Id = subjects.Subjects_Id;
            res.Subjects_Name = subjects.Subjects_Name;
            if (subjects.Subjects_User != null)
            {
                res.Teacher = UserSimplifyDto.FromUser(subjects.Subjects_User);
            }
            return res;
        }

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

        public int Presence_id { get; set; }
    }

}
