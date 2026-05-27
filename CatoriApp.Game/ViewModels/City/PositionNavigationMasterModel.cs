using CatoriApp.Core.Objects;
namespace CatoriApp.Game.ViewModels.City
{
    public class PositionNavigationMasterModel
    {
        public List<PathPositionModel> PathPositions { get; set; }  = new List<PathPositionModel>();
        public string PositionName { get; set; }
    }
}


