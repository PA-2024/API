using System.ComponentModel.DataAnnotations;
using System.Runtime.InteropServices.Marshalling;

namespace API_GesSIgn.Models
{
    /// <summary>
    /// différente acces 
    /// </summary>
    public class School
    {
        [Key]
        public  int School_Id { get; set; }

        [MaxLength(255)]
        [Required]
        public string School_Name { get; set; }

        public string? School_token { get; set; }

        [Required]
        public bool School_allowSite { get; set; }

        [Required]
        public DateTime School_Date { get; set; }

    }
}
