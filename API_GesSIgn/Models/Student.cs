using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace API_GesSIgn.Models
{
    /// <summary>
    /// Classe des étudiants d'une école
    /// liée forcement à un utilisateur
    /// </summary>
    public class Student
    {
        [Key]
        public int Student_Id { get; set; }

        [Required]
        public string Student_FirstName { get; set; }

        [Required]
        public string Student_LastName { get; set; }

        [Required]
        public int Student_User_Id { get; set; }

        [ForeignKey("Student_User_Id")]
        public User Student_User { get; set; }

        [Required]
        public int Student_Sector_Id { get; set; }

        [ForeignKey("Student_Sector_Id")]
        public Sectors Student_Sectors { get; set; }
    }
}
