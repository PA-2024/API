namespace API_GesSIgn.Models
{
    /// <summary>
    /// Class des étudiant d'une école
    /// lié forcement a un utilisateurs 
    /// </summary>
    public class Student
    {
        public int Student_Id { get; set; }

        public int Student_FirstName { get; set; }

        public int Student_LastName { get; set; }

        public required User Student_User { get; set; }

        /// <summary>
        /// Classe de l'etudiant
        /// </summary>
        public required Sectors Student_sectors { get; set; }


    }
}