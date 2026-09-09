namespace CatoriApp.Game.ViewModels.Manufacturing
{
    public class MachineTypeViewModel : ViewmodelBase
    {
        private int _machineTypeId;
        private string _name = "";

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
    }
}


