using System.ComponentModel.DataAnnotations;

namespace API_GesSIgn.Models
{
    /// <summary>
    /// Class des étudiant d'une école
    /// lié forcement a un utilisateurs 
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
        public  User Student_User { get; set; }

        /// <summary>
        /// Classe de l'etudiant
        /// </summary>
        [Required]
        public Sectors Student_sectors { get; set; }


    }
}