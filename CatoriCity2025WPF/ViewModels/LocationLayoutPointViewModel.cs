namespace CatoriApp.ViewModels
{
    public class LocationLayoutPointViewModel : ViewmodelBase
    {
        private long _locationLayoutPointId;
        private long _locationLayoutItemId;
        private int _pointIndex;
        private string? _pointRole;
        private double _x;
        private double _y;
        private double _z;
        private LocationLayoutSegmentKind _segmentKind = LocationLayoutSegmentKind.Line;
        private double? _control1X;
        private double? _control1Y;
        private double? _control2X;
        private double? _control2Y;
        private double? _rotationDegrees;

        public long LocationLayoutPointId { get => _locationLayoutPointId; set => SetProperty(ref _locationLayoutPointId, value); }
        public long LocationLayoutItemId { get => _locationLayoutItemId; set => SetProperty(ref _locationLayoutItemId, value); }
        public int PointIndex { get => _pointIndex; set => SetProperty(ref _pointIndex, value); }
        public string? PointRole { get => _pointRole; set => SetProperty(ref _pointRole, value); }
        public double X { get => _x; set => SetProperty(ref _x, value); }
        public double Y { get => _y; set => SetProperty(ref _y, value); }
        public double Z { get => _z; set => SetProperty(ref _z, value); }
        public LocationLayoutSegmentKind SegmentKind { get => _segmentKind; set => SetProperty(ref _segmentKind, value); }
        public double? Control1X { get => _control1X; set => SetProperty(ref _control1X, value); }
        public double? Control1Y { get => _control1Y; set => SetProperty(ref _control1Y, value); }
        public double? Control2X { get => _control2X; set => SetProperty(ref _control2X, value); }
        public double? Control2Y { get => _control2Y; set => SetProperty(ref _control2Y, value); }
        public double? RotationDegrees { get => _rotationDegrees; set => SetProperty(ref _rotationDegrees, value); }
    }
}

