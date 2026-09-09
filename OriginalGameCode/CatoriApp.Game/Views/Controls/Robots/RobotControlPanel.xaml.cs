namespace CatoriApp.Game.Views.Controls.Robots
{
    /// <summary>
    /// Interaction logic for RobotControlPanel.xaml
    /// </summary>
    public partial class RobotControlPanel : UserControl
    {
        public event EventHandler<RobotControlPanelSelectionsArg> RunRequested;
        public event EventHandler? DesignModeRequested;
        public event EventHandler? DesignModeEndRequested;
        public event EventHandler? TeachRequested;
        private List<string> _secondaryItems;
        bool _DesignMode = false;
        public long LocationId { get; set; }
        public string LocationBackgroundImagePath { get; set; } = "";
        public RobotControlPanel()
        {
            InitializeComponent();
            UpdateStatus("Idle");
        }
        public string SelectedInputA => InputAComboBox.SelectedItem?.ToString() ?? "";
        public string SelectedInputB => InputBComboBox.SelectedItem?.ToString() ?? "";
        public string SelectedOutput => OutputComboBox.SelectedItem?.ToString() ?? "";


        public void LoadData(
            string robotName,
            List<string> PrimaryItems,
            List<string> SecondaryItems,
            List<string> outputItems
           )
        {
            RobotNameTextBlock.Text = robotName;
            _secondaryItems = SecondaryItems;
            LoadComboBox(InputAComboBox, PrimaryItems);
            LoadComboBox(OutputComboBox, outputItems);
        }

        private void LoadComboBox(ComboBox comboBox, List<string> items)
        {
            comboBox.ItemsSource = null;
            comboBox.Items.Clear();

            foreach (string item in items)
            {
                comboBox.Items.Add(item);
            }

            if (comboBox.Items.Count > 0)
                comboBox.SelectedIndex = 0;
        }

        private void RunButton_Click_1(object sender, RoutedEventArgs e)
        {
            RobotControlPanelSelectionsArg args = new RobotControlPanelSelectionsArg();
            args.InputA = SelectedInputA;
            args.InputB = SelectedInputB;
            args.Output = SelectedOutput;
            RunRequested(this, args);
        }
        private void TeachButton_Click(object sender, RoutedEventArgs e)
        {
            UpdateStatus("Teach mode started. Click pickup/drop locations.");
            TeachRequested?.Invoke(this, EventArgs.Empty);
        }

        public void UpdateStatus(string message)
        {
            StatusTextBlock.Text = message;
        }
        string Designbuttontext = "Design";
        private void DesignModeButton_Click(object sender, RoutedEventArgs e)
        {
            if (_DesignMode)
            {
                UpdateStatus("Design mode disabled");
                DesignModeButton.Content = Designbuttontext;
                RunButton.IsEnabled = true;
                _DesignMode = false;
                DesignModeEndRequested(this, e);
                RunButton.IsEnabled = true;
            }
            else
            {
                Designbuttontext = DesignModeButton.Content.ToString();
                DesignModeButton.Content = "End Design";
                UpdateStatus("Design mode enabled. Click to change inputs/outputs.");
                DesignModeRequested(this, e);
                RunButton.IsEnabled = false;
                _DesignMode = true;
                RunButton.IsEnabled = false;
            }
        }

        private void InputAComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            List<string> list = new List<string>();
            string selectedA = InputAComboBox.SelectedValue.ToString();
            var found = from f in _secondaryItems where f != selectedA select f;
            List<string> foundItems = new List<string>();
            foreach (var item in found)
            {
                foundItems.Add(item);
            }
            LoadComboBox(InputBComboBox, foundItems);

        }

        private void RobotDesignButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Rect sourcerectangle = new Rect(0, 0, 500, 500);
                var view = new MachineLayoutDesignerSelectionService().CreateDesignerWindow(LocationId, LocationBackgroundImagePath, sourcerectangle, CatoriApp.MachineLayoutDesigner.Objects.Enums.MachineDesignerModeEnum.RobotArm);
                view.Owner = Window.GetWindow(this);
                view.Show();
            }
            catch (Exception ex)
            {

                throw;
            }
        }
    }
}



