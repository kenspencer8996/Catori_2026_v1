using CatoriApp.Objects;

namespace CatoriApp.ViewModels
{
    public class PositionNavigationMasterModel
    {
        public List<PathPositionModel> PathPositions { get; set; }  = new List<PathPositionModel>();
        public string PositionName { get; set; }
    }
}

