namespace API_GesSIgn.Models
{
    /// <summary>
    /// Classe des matières 
    /// </summary>
    public class Subjects
    {
        public required int Subjects_Id { get; set; }

        public required User Subjects_User { get; set; }

        public required Sectors Subjects_Sectors { get; set; }  
      
    }
}
