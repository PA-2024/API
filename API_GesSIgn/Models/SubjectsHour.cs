namespace API_GesSIgn.Models
{
    /// <summary>
    /// Classe des matières 
    /// </summary>
    public class SubjectsHour
    {
        public required int SubjectsHour_Id { get; set; }

        public required Sectors SubjectsHour_Sectors { get; set; }  

        public string? SubjectsHour_Rooom { get; set; } 

        public DateTime SubjectsHour_Date { get; set; } 
      
    }
}