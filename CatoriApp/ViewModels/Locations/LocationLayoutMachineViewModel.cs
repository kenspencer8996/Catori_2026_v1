namespace CatoriApp.ViewModels.Locations
{
    public class LocationLayoutMachineViewModel : ViewmodelBase
    {
        private int _locationLayoutMachineId;
        private int _locationId;
        private int _machineId;
        private double _x;
        private double _y;
        private double? _width;
        private double? _height;
        private double _rotation;
        private int _zIndex = 100;
        private bool _isEnabled = true;

        public int LocationLayoutMachineId
        {
            get => _locationLayoutMachineId;
            set => SetProperty(ref _locationLayoutMachineId, value);
        }

        public int LocationId
        {
            get => _locationId;
            set => SetProperty(ref _locationId, value);
        }

        public int MachineId
        {
            get => _machineId;
            set => SetProperty(ref _machineId, value);
        }

        public double X
        {
            get => _x;
            set => SetProperty(ref _x, value);
        }

        public double Y
        {
            get => _y;
            set => SetProperty(ref _y, value);
        }

        public double? Width
        {
            get => _width;
            set => SetProperty(ref _width, value);
        }

        public double? Height
        {
            get => _height;
            set => SetProperty(ref _height, value);
        }

        public double Rotation
        {
            get => _rotation;
            set => SetProperty(ref _rotation, value);
        }

        public int ZIndex
        {
            get => _zIndex;
            set => SetProperty(ref _zIndex, value);
        }

        public bool IsEnabled
        {
            get => _isEnabled;
            set => SetProperty(ref _isEnabled, value);
        }
    }
}


