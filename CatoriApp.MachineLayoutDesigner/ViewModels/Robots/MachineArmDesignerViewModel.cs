namespace CatoriApp.MachineLayoutDesigner.ViewModels.Robots
{
    public class MachineArmDesignerViewModel : ViewmodelBase
    {
        private MachineDefinitionViewModel _definition = new();
        private MachineInstanceViewModel _instance = new();
        private string _statusMessage = "";
        private double _joint1 = -90;
        private double _joint2;
        private double _joint3;
        private double _jointEnd;

        public MachineDefinitionViewModel Definition
        {
            get => _definition;
            set => SetProperty(ref _definition, value);
        }

        public MachineInstanceViewModel Instance
        {
            get => _instance;
            set => SetProperty(ref _instance, value);
        }

        public double Joint1 { get => _joint1; set => SetProperty(ref _joint1, value); }
        public double Joint2 { get => _joint2; set => SetProperty(ref _joint2, value); }
        public double Joint3 { get => _joint3; set => SetProperty(ref _joint3, value); }
        public double JointEnd { get => _jointEnd; set => SetProperty(ref _jointEnd, value); }

        public string StatusMessage
        {
            get => _statusMessage;
            set => SetProperty(ref _statusMessage, value);
        }
    }
}
