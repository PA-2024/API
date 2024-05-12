using System.ComponentModel.DataAnnotations;

namespace API_GesSIgn.Models
{
    public class Roles
    {
        [Key]
        public int Roles_Id { get; set; }

        [Required]
        public string Role_Name { get; set; }

    }
}
