namespace API_GesSIgn.Models
{
    /// <summary>
    /// différent batiment de l'école  
    /// </summary>
    public class Presence
    {
        public required int Presence_Id { get; set; }

        public required User Presence_User { get; set; }

        public required User Presence_SubjectsHour { get; set; }

    }
}
