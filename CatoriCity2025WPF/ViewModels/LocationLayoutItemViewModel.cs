using System.Collections.ObjectModel;

namespace CatoriApp.ViewModels
{
    public class LocationLayoutItemViewModel : ViewmodelBase
    {
        private long _locationLayoutItemId;
        private long _locationLayoutId;
        private string _itemName = "";
        private LocationLayoutItemType _itemType = LocationLayoutItemType.Conveyor;
        private double _x;
        private double _y;
        private double _z;
        private double _width;
        private double _height;
        private double _rotationDegrees;
        private int _zIndex;
        private bool _isLocked;
        private string? _imagePath;
        private string? _metadataJson;

        public long LocationLayoutItemId { get => _locationLayoutItemId; set => SetProperty(ref _locationLayoutItemId, value); }
        public long LocationId { get => _locationLayoutId; set => SetProperty(ref _locationLayoutId, value); }
        public string ItemName { get => _itemName; set => SetProperty(ref _itemName, value); }
        public LocationLayoutItemType ItemType { get => _itemType; set => SetProperty(ref _itemType, value); }
        public double X { get => _x; set => SetProperty(ref _x, value); }
        public double Y { get => _y; set => SetProperty(ref _y, value); }
        public double Z { get => _z; set => SetProperty(ref _z, value); }
        public double Width { get => _width; set => SetProperty(ref _width, value); }
        public double Height { get => _height; set => SetProperty(ref _height, value); }
        public double RotationDegrees { get => _rotationDegrees; set => SetProperty(ref _rotationDegrees, value); }
        public int ZIndex { get => _zIndex; set => SetProperty(ref _zIndex, value); }
        public bool IsLocked { get => _isLocked; set => SetProperty(ref _isLocked, value); }
        public string? ImagePath { get => _imagePath; set => SetProperty(ref _imagePath, value); }
        public string? MetadataJson { get => _metadataJson; set => SetProperty(ref _metadataJson, value); }
        public ObservableCollection<LocationLayoutPointViewModel> Points { get; } = new();
    }
}

