namespace API_GesSIgn.Models
{
    /// <summary>
    /// Class pour les utilisateurs S
    /// </summary>
    public class User
    {
        public required int User_Id { get; set; }

        public required string User_email { get; set; }

        public required string User_password { get; set; }
    }
}
