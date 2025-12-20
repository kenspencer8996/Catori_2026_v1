using CatoriCity2025WPF.Objects;

namespace CatoriCity2025WPF.ViewModels
{
    public class PositionNavigationMasterModel
    {
        public List<PathPositionModel> PathPositions { get; set; }  = new List<PathPositionModel>();
        public string PositionName { get; set; }
    }
}
