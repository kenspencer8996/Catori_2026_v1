namespace CatoriCity2025WPF.ViewModels
{
    public class FactoryLayoutPointViewModel : ViewmodelBase
    {
        private long _factoryLayoutPointId;
        private long _factoryLayoutItemId;
        private int _pointIndex;
        private string? _pointRole;
        private double _x;
        private double _y;
        private double _z;
        private FactoryLayoutSegmentKind _segmentKind = FactoryLayoutSegmentKind.Line;
        private double? _control1X;
        private double? _control1Y;
        private double? _control2X;
        private double? _control2Y;
        private double? _rotationDegrees;

        public long FactoryLayoutPointId { get => _factoryLayoutPointId; set => SetProperty(ref _factoryLayoutPointId, value); }
        public long FactoryLayoutItemId { get => _factoryLayoutItemId; set => SetProperty(ref _factoryLayoutItemId, value); }
        public int PointIndex { get => _pointIndex; set => SetProperty(ref _pointIndex, value); }
        public string? PointRole { get => _pointRole; set => SetProperty(ref _pointRole, value); }
        public double X { get => _x; set => SetProperty(ref _x, value); }
        public double Y { get => _y; set => SetProperty(ref _y, value); }
        public double Z { get => _z; set => SetProperty(ref _z, value); }
        public FactoryLayoutSegmentKind SegmentKind { get => _segmentKind; set => SetProperty(ref _segmentKind, value); }
        public double? Control1X { get => _control1X; set => SetProperty(ref _control1X, value); }
        public double? Control1Y { get => _control1Y; set => SetProperty(ref _control1Y, value); }
        public double? Control2X { get => _control2X; set => SetProperty(ref _control2X, value); }
        public double? Control2Y { get => _control2Y; set => SetProperty(ref _control2Y, value); }
        public double? RotationDegrees { get => _rotationDegrees; set => SetProperty(ref _rotationDegrees, value); }
    }
}
