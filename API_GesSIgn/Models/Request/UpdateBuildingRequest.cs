using System.ComponentModel.DataAnnotations;

namespace API_GesSIgn.Models.Request
{

    public class UpdateBuildingRequest
    {
        [Required]
        public string Bulding_City { get; set; }

        [Required]
        public string Bulding_Name { get; set; }

        [Required]
        public string Bulding_Adress { get; set; }
    }

}