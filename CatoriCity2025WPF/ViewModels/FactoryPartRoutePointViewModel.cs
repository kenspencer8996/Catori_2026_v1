namespace CatoriCity2025WPF.ViewModels
{
    public class FactoryPartRoutePointViewModel : ViewmodelBase
    {
        private long _factoryPartRoutePointId;
        private long _factoryPartRouteId;
        private int _pointIndex;
        private double _x;
        private double _y;
        private double _z;
        private double _secondsFromStart;

        public long FactoryPartRoutePointId { get => _factoryPartRoutePointId; set => SetProperty(ref _factoryPartRoutePointId, value); }
        public long FactoryPartRouteId { get => _factoryPartRouteId; set => SetProperty(ref _factoryPartRouteId, value); }
        public int PointIndex { get => _pointIndex; set => SetProperty(ref _pointIndex, value); }
        public double X { get => _x; set => SetProperty(ref _x, value); }
        public double Y { get => _y; set => SetProperty(ref _y, value); }
        public double Z { get => _z; set => SetProperty(ref _z, value); }
        public double SecondsFromStart { get => _secondsFromStart; set => SetProperty(ref _secondsFromStart, value); }
    }
}
