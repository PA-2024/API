using System.ComponentModel.DataAnnotations;

namespace API_GesSIgn.Models
{
    /// <summary>
    /// Classe des étudients 
    /// </summary>
    public class Sectors
    {
        [Key]
        public int Sectors_Id { get; set; }

        [Required]
        public string Sectors_Name { get; set; }

    }
}
