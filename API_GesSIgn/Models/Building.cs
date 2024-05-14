using System.ComponentModel.DataAnnotations;
using System.Runtime.InteropServices.Marshalling;

namespace API_GesSIgn.Models
{
    /// <summary>
    /// différent batiment de l'école  
    /// </summary>
    public class Building
    {
        [Key]
        public int Bulding_Id { get; set; }

        [Required]
        public string Bulding_City {get; set;}

        [Required]
        public string Bulding_Name {get; set;}

        [Required]
        public string Bulding_Adress {get; set;}

        [Required]
        public School School { get; set;}

    }
}
