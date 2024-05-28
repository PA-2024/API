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
        public int Student_User_Id { get; set; }

        /// <summary>
        /// lien entre l'etudiant et l'user 
        /// </summary>
        [ForeignKey("Student_User_Id")]
        public User Student_User { get; set; }

        
        [Required]
        public int Student_Sector_Id { get; set; }
        
        /// <summary>
        /// Classe de l'étudiant 
        /// </summary>
        [ForeignKey("Student_Sector_Id")]
        public Sectors Student_Sectors { get; set; }
    }
}
