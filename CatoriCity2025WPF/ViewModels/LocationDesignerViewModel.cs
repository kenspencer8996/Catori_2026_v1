using System.Collections.ObjectModel;

namespace CatoriApp.ViewModels
{
    public class LocationDesignerViewModel : ViewmodelBase
    {
        private int _locationId;
        private string _locationName = "";
        private string _layoutName = "";
        private string? _backgroundImagePath;
        private double _canvasWidth = 1920;
        private double _canvasHeight = 1080;
        private LocationLayoutItemViewModel? _selectedItem;

        public int LocationId { get => _locationId; set => SetProperty(ref _locationId, value); }
        public string LocationName { get => _locationName; set => SetProperty(ref _locationName, value); }
        public string LayoutName { get => _layoutName; set => SetProperty(ref _layoutName, value); }
        public string? BackgroundImagePath { get => _backgroundImagePath; set => SetProperty(ref _backgroundImagePath, value); }
        public double CanvasWidth { get => _canvasWidth; set => SetProperty(ref _canvasWidth, value); }
        public double CanvasHeight { get => _canvasHeight; set => SetProperty(ref _canvasHeight, value); }
        public LocationLayoutItemViewModel? SelectedItem { get => _selectedItem; set => SetProperty(ref _selectedItem, value); }
        public ObservableCollection<LocationLayoutItemViewModel> Items { get; } = new();
        public ObservableCollection<LocationPartRouteViewModel> Routes { get; } = new();
    }
}

