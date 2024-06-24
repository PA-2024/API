using System.ComponentModel.DataAnnotations;

namespace API_GesSIgn.Models.Request
{
    public class StudentRequest
    {
        public int Student_User_id { get; set; }

        public int Student_Class_id { get; set; }
    }

    public class UpdateStudentSectorRequest
    {       
        [Required]
        public int Student_Sector_Id { get; set; }
    }

}
