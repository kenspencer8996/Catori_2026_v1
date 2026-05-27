using CatoriApp.Core.Objects;
namespace CatoriApp.Game.ViewModels.Locations
{
    public class CampfireSpotLocationViewmodel
    {
        public CampfireSpotLocationViewmodel(string SpotLocationName ) 
        {
            LocationName = SpotLocationName;
        }
        public BusinessControlAndLocXYEntity Location { get; set; } = new BusinessControlAndLocXYEntity();
        public int CampfireSpotNumber { get; set; }
        public string RobberName { get; set; } = "";
        public string LocationName { get; set; } = "";
        public int Degrees { get; set; }
    }
}


