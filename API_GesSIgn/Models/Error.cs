namespace API_GesSIgn.Models
{
    /// <summary>
    /// Permet une historisation des defférentes erreur soulevé dans l'api 
    /// </summary>
    public class Error 
    {
        public required int Error_id {get; set;}

        public string? Error_Funtion {get; set;}

        public required DateTime Error_DateTime {get; set;}
    }
}