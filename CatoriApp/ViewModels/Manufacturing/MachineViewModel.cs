namespace CatoriApp.ViewModels.Manufacturing
{
    public class MachineViewModel : ViewmodelBase
    {
        private int _machineId;
        private int _machineTypeId;
        private string _name = "";
        private string? _imagePath;
        private string? _controlTypeName;
        private string? _description;

        public int MachineId
        {
            get => _machineId;
            set => SetProperty(ref _machineId, value);
        }

        public int MachineTypeId
        {
            get => _machineTypeId;
            set => SetProperty(ref _machineTypeId, value);
        }

        public string Name
        {
            get => _name;
            set => SetProperty(ref _name, value);
        }

        public string? ImagePath
        {
            get => _imagePath;
            set => SetProperty(ref _imagePath, value);
        }

        public string? ControlTypeName
        {
            get => _controlTypeName;
            set => SetProperty(ref _controlTypeName, value);
        }

        public string? Description
        {
            get => _description;
            set => SetProperty(ref _description, value);
        }
    }
}


