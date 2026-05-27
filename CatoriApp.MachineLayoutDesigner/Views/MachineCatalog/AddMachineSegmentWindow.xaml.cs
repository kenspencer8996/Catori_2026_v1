using CatoriApp.Core.Objects.Shared;

namespace CatoriApp.MachineLayoutDesigner.Views.MachineCatalog
{
    /// <summary>
    /// Interaction logic for AddMachineSegmentWindow.xaml
    /// </summary>
    public partial class AddMachineSegmentWindow : Window
    {
        public AddMachineSegmentWindow()
        {
            InitializeComponent();
            LoadArmTypes("Default");
        }

        private List<string> _armTypes = new List<string>();
        private void LoadArmTypes(string armTypes)
        {
            switch (armTypes)
            {
                case "Default":
                    _armTypes = new List<string> 
                    { "Base", "ArmShort", "ArmMedium", "ArmLong", "Hand" };
                    break;
                default:
                    break;
            }
            ArmTypeComboBox.ItemsSource = _armTypes;
        }

        private void ArmColorComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var selectedArmType = ArmTypeComboBox.SelectedItem as string;
            if (selectedArmType != null && selectedArmType != "")
            {
                var selectedArmColor = ArmColorComboBox.SelectedItem as string;
                string localimagePath = "";
                string baseimagepath = System.IO.Path.Combine(GlobalAllApps.ImageFolder, "Factories\\RobotArms");
                localimagePath = System.IO.Path.Combine(baseimagepath, selectedArmType, selectedArmColor, ".png");
                var ImageSourceOpen = UIUtility.GetImageControl(baseimagepath, 100, 100, 3010);
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
    }
}
