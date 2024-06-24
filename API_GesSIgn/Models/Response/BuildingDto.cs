namespace API_GesSIgn.Models.Response
{
    /// <summary>
    /// Classe de réponse pour les bâtiments
    /// </summary>
    public class BuildingDto
    {
        /// <summary>
        /// id du bâtiment
        /// </summary>
        public int Building_Id { get; set; }

        /// <summary>
        /// Nom du bâtiment
        /// </summary>
        public string Building_Name { get; set; }

        /// <summary>
        /// Ville du bâtiment
        /// </summary>
        public string Building_City { get; set; }

        /// <summary>
        /// Adresse du bâtiment
        /// </summary>
        public string Building_Address { get; set; }

        public static BuildingDto FromBuilding(Building building)
        {
            return new BuildingDto
            {
                Building_Id = building.Bulding_Id,
                Building_Name = building.Bulding_Name,
                Building_City = building.Bulding_City,
                Building_Address = building.Bulding_Adress
            };
        }
    }

}
