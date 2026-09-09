using CatoriApp.Core.Objects.Shared;

namespace CatoriApp.MachineLayoutDesigner.Views.MachineCatalog
{
    /// <summary>
    /// Interaction logic for AddMachineSegmentWindow.xaml
    /// </summary>
    public partial class MachineSegmentEditEditorView : Window
    {
        public MachineInstanceSegmentViewModel model;
        public MachineSegmentEditEditorView(MachineInstanceSegmentViewModel viewModel)
        {
            InitializeComponent();
            LoadArmMetadatas("Default");
            model = viewModel;
            //model.ar
        }

        private void LoadArmMetadatas(string armTypes)
        {
          List<string> _armTypes = new List<string>();
          List<string> _armColors = new List<string>();
          List<string> _armLengths = new List<string>();
            switch (armTypes)
            {
                case "Default":
                    _armTypes = new List<string> 
                    { "Base", "ArmShort", "ArmMedium", "ArmLong", "Hand" };
                    _armColors = new List<string>
                    {
                    "Blue","Red","Yellow","Steel","Black"
                    };
                    break;
                default:
                    break;
            }

            ArmTypeComboBox.ItemsSource = _armTypes;
            ArmColorComboBox.ItemsSource = _armColors;
            _armLengths = new List<string>
                    {
                     "80", "124", "160", "220"
                    };
            ArmLengthComboBox.ItemsSource = _armLengths;
        }

        private void ArmColorComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var selectedArmType = ArmTypeComboBox.SelectedItem as string;

            if (selectedArmType != null && selectedArmType != "")
            {
             
                var selectedArmColor = ArmColorComboBox.SelectedItem as string;
                string localimagePath = "";
                string baseimagepath = System.IO.Path.Combine(GlobalAllApps.ImageFolder, "Factories\\RobotArms");
                localimagePath = System.IO.Path.Combine(baseimagepath, "robotArm" + selectedArmType + selectedArmColor, ".png");
                var ImageSourceOpen = UIUtility.GetImageControl(localimagePath, 100, 100, 3010);
                PreviewImage.Source = ImageSourceOpen.Source;
            }
        }

        private void ArmTypeComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void ArmLengthComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            this.Close();
        }
    }
}
