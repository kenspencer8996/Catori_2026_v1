using System.Collections.ObjectModel;
namespace CatoriApp.Game.ViewModels.Locations
{
    public class LocationLayoutItemViewModel : ViewmodelBase
    {
        private long _locationLayoutItemId;
        private long _locationId;
        private string _itemName = "";
        private LocationLayoutItemType _itemType = LocationLayoutItemType.Conveyor;
        private string? _majorItemType;
        private double _x;
        private double _y;
        private double _z;
        private double _width;
        private double _height;
        private double _rotationDegrees;
        private int _zIndex;
        private bool _isLocked;
        private string? _itemDataJson;
        private string? _metadataJson;

        public long LocationLayoutItemId { get => _locationLayoutItemId; set => SetProperty(ref _locationLayoutItemId, value); }
        public long LocationId { get => _locationId; set => SetProperty(ref _locationId, value); }
        public string ItemName { get => _itemName; set => SetProperty(ref _itemName, value); }
        public LocationLayoutItemType ItemType { get => _itemType; set => SetProperty(ref _itemType, value); }
        public string? MajorItemType { get => _majorItemType; set => SetProperty(ref _majorItemType, value); }
        public double X { get => _x; set => SetProperty(ref _x, value); }
        public double Y { get => _y; set => SetProperty(ref _y, value); }
        public double Z { get => _z; set => SetProperty(ref _z, value); }
        public double Width { get => _width; set => SetProperty(ref _width, value); }
        public double Height { get => _height; set => SetProperty(ref _height, value); }
        public double RotationDegrees { get => _rotationDegrees; set => SetProperty(ref _rotationDegrees, value); }
        public int ZIndex { get => _zIndex; set => SetProperty(ref _zIndex, value); }
        public bool IsLocked { get => _isLocked; set => SetProperty(ref _isLocked, value); }
        public string? ItemDataJson { get => _itemDataJson; set => SetProperty(ref _itemDataJson, value); }
        public string? MetadataJson { get => _metadataJson; set => SetProperty(ref _metadataJson, value); }
        public ObservableCollection<LocationLayoutPointViewModel> Points { get; } = new();

        public LocationLayoutItemEntity ToEntity()
        {
            var itemDataJson = string.IsNullOrWhiteSpace(ItemDataJson)
                ? null
                : ItemDataJson.Trim();

            return new LocationLayoutItemEntity
            {
                LocationLayoutItemId = LocationLayoutItemId,
                LocationId = LocationId,
                ItemName = ItemName.Trim(),
                ItemType = ItemType,
                MajorItemType = string.IsNullOrWhiteSpace(MajorItemType)
                    ? null
                    : MajorItemType.Trim(),
                X = X,
                Y = Y,
                Z = Z,
                Width = Width,
                Height = Height,
                RotationDegrees = RotationDegrees,
                ZIndex = ZIndex,
                IsLocked = IsLocked,
                ItemDataJson = itemDataJson,
                MetadataJson = MetadataJson,
                Points = Points.Select(point => new LocationLayoutPointEntity
                {
                    LocationLayoutPointId = point.LocationLayoutPointId,
                    LocationLayoutItemId = point.LocationLayoutItemId,
                    LocationId = point.LocationId,
                    PointIndex = point.PointIndex,
                    PointRole = point.PointRole,
                    X = point.X,
                    Y = point.Y,
                    Z = point.Z,
                    SegmentKind = point.SegmentKind,
                    Control1X = point.Control1X,
                    Control1Y = point.Control1Y,
                    Control2X = point.Control2X,
                    Control2Y = point.Control2Y,
                    RotationDegrees = point.RotationDegrees
                }).ToList()
            };
        }
    }
}


