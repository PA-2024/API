namespace API_GesSIgn.Models
{
    /// <summary>
    /// Class lié au QCM
    /// </summary>
    public class QCM 
    {
        public required User QCM_Teacher {get; set;}

        public required bool QCM_Done {get; set;}
    }
}