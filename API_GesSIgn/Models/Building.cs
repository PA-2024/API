using System.Runtime.InteropServices.Marshalling;

namespace API_GesSIgn.Models
{
    /// <summary>
    /// différent batiment de l'école  
    /// </summary>
    public class Bulding
    {
        public required int Bulding_Id { get; set; }

        public required string Bulding_City {get; set;}

        public required string Bulding_Name {get; set;}

        public required string Bulding_Adress {get; set;}

    }
}
