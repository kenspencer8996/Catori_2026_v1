using System.Collections.ObjectModel;
namespace CatoriApp.Game.ViewModels.Locations
{
    public class LocationViewModel : ViewmodelBase
    {
        public int LocationId { get; set; }
        public string LocationName { get; set; } = "";
        public string? Description { get; set; }
        public string BackgroundImagePath { get; set; } = "";
        public string InteriorType { get; set; } = "";
        public string? WorldMapImagePath { get; set; }
        public double HotspotLeft { get; set; }
        public double HotspotTop { get; set; }
        public double HotspotWidth { get; set; } = 100;
        public double HotspotHeight { get; set; } = 100;
        public double DesignWidth { get; set; } = 1920;
        public double DesignHeight { get; set; } = 1080;
        public double? DefaultRobotX { get; set; }
        public double? DefaultRobotY { get; set; }
        public bool IsActive { get; set; } = true;
        public int SortOrder { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime? UpdatedDate { get; set; }
        public List<LocationLayoutPointEntity> Points { get; set; } = new();
        public ObservableCollection<RobotPoseViewModel> Poses { get; set; } = new();

    }
}


