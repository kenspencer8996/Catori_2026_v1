namespace CatoriApp.ViewModels
{
    public class LocationPartRoutePointViewModel : ViewmodelBase
    {
        private long _locationPartRoutePointId;
        private long _locationPartRouteId;
        private int _pointIndex;
        private double _x;
        private double _y;
        private double _z;
        private double _secondsFromStart;

        public long LocationPartRoutePointId { get => _locationPartRoutePointId; set => SetProperty(ref _locationPartRoutePointId, value); }
        public long LocationPartRouteId { get => _locationPartRouteId; set => SetProperty(ref _locationPartRouteId, value); }
        public int PointIndex { get => _pointIndex; set => SetProperty(ref _pointIndex, value); }
        public double X { get => _x; set => SetProperty(ref _x, value); }
        public double Y { get => _y; set => SetProperty(ref _y, value); }
        public double Z { get => _z; set => SetProperty(ref _z, value); }
        public double SecondsFromStart { get => _secondsFromStart; set => SetProperty(ref _secondsFromStart, value); }
    }
}

