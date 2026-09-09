namespace CatoriApp.Core.ViewModels.Robots
{
    public class MachineInstanceSegmentViewModel : ViewmodelBase
    {
        private long _machineInstanceSegmentId;
        private long _machineInstanceId;
        private int _segmentIndex;
        private string _segmentName = "";
        private double _length = 124;
        private double _width = 40;
        private double _initialAngle;
        private double _minAngle = -180;
        private double _maxAngle = 180;
        private double _overlap = 16;
        private string _color = "Blue";
        private string _partType = "long";
        private string _imageName = "";

        public long MachineInstanceSegmentId { get => _machineInstanceSegmentId; set => SetProperty(ref _machineInstanceSegmentId, value); }
        public long MachineInstanceId { get => _machineInstanceId; set => SetProperty(ref _machineInstanceId, value); }
        public int SegmentIndex { get => _segmentIndex; set => SetProperty(ref _segmentIndex, value); }
        public string SegmentName { get => _segmentName; set => SetProperty(ref _segmentName, value ?? ""); }
        public double Length { get => _length; set => SetProperty(ref _length, value); }
        public double Width { get => _width; set => SetProperty(ref _width, value); }
        public double InitialAngle { get => _initialAngle; set => SetProperty(ref _initialAngle, value); }
        public double MinAngle { get => _minAngle; set => SetProperty(ref _minAngle, value); }
        public double MaxAngle { get => _maxAngle; set => SetProperty(ref _maxAngle, value); }
        public double Overlap { get => _overlap; set => SetProperty(ref _overlap, value); }
        public string Color { get => _color; set => SetProperty(ref _color, value ?? ""); }
        public string PartType { get => _partType; set => SetProperty(ref _partType, value ?? ""); }
        public string ImageName { get => _imageName; set => SetProperty(ref _imageName, value ?? ""); }
    }
}