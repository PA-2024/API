using System.Runtime.InteropServices.Marshalling;

namespace API_GesSIgn.Models
{
    /// <summary>
    /// différente acces 
    /// </summary>
    public class School
    {
        public required int School_Id { get; set; }

        public required string School_Name { get; set; }

        public string? School_token { get; set; }

        public required bool School_allowSite { get; set; }

    }
}
