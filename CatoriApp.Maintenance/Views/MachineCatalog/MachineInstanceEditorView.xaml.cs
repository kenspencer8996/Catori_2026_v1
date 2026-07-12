using CatoriApp.Core.Objects.Shared;
using System.Diagnostics.Metrics;

namespace CatoriApp.MachineLayoutDesigner.Views.MachineCatalog
{
     /// <summary>
    /// Interaction logic for MachineInstanceEditorView.xaml
    /// </summary>
    public partial class MachineInstanceEditorView : Window
    {
        private readonly MachineInstanceViewModel _instance;
        private readonly MachineInstanceEditorViewModel _viewModel = new();
        private readonly MachineCatalogService _service = new();
        List<MachineInstanceViewModel> _InstaneList;
        public MachineInstanceEditorView(MachineInstanceViewModel instance)
        {
            _instance = instance;
            InitializeComponent();

            DataContext = _instance;
        }
        private void SetUIValues()
        {
            if (_instance != null && _instance.InstanceName != "")
            {
                DefaultScaleTextBox.Text = _instance.DefaultScale.ToString();
                DefaultWidthTextBox.Text = _instance.DefaultWidth.ToString();
                DefaultHeightTextBox.Text = _instance.DefaultHeight.ToString();
              }
        }
        private void SetInstanceValues()
        {
            if (_instance != null && _instance.InstanceName != "")
            {
                _instance.DefaultScale =CatoriStringConverter.ConvertToDouble( DefaultScaleTextBox.Text);
                _instance.DefaultWidth = CatoriStringConverter.ConvertToDouble(DefaultWidthTextBox.Text);
                _instance.DefaultHeight =CatoriStringConverter.ConvertToDouble( DefaultHeightTextBox.Text);

            }
        }
       
        private void DefaultHeightSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (DefaultHeightTextBox.Text != DefaultHeightSlider.Value.ToString())
                DefaultHeightTextBox.Text = DefaultHeightSlider.Value.ToString();

        }

        private void DefaultWidthSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {

            if (DefaultWidthTextBox.Text != DefaultWidthSlider.Value.ToString())
                DefaultWidthTextBox.Text = DefaultWidthSlider.Value.ToString();
        }

  
        private void DefaultHeightTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            double def = CatoriStringConverter.ConvertToDouble(DefaultHeightTextBox.Text);
            if (DefaultHeightSlider.Value != def)
                DefaultHeightSlider.Value = def;
        }

        private void DefaultWidthTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            double def = CatoriStringConverter.ConvertToDouble(DefaultWidthTextBox.Text);
            if (DefaultWidthSlider.Value != def)
                DefaultWidthSlider.Value = def;

        }

       

        private void DefaultScaleTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            double def = CatoriStringConverter.ConvertToDouble(DefaultScaleTextBox.Text);
           
        }

        private async Task LoadCatalogAsync()
        {
            _viewModel.Definitions.Clear();
            _viewModel.Instances.Clear();

            foreach (var definition in await _service.GetAllDefinitionsAsync())
                _viewModel.Definitions.Add(definition);

            _InstaneList = _service.GetAllInstancesAsync().Result;
    
            _viewModel.StatusMessage = "Machine items loaded.";

            MachineCatalogComboBox.ItemsSource = _viewModel.Definitions;
            //if (_viewModel.Instances.Count == 0)
            //{
            //    CreateNewInstance();
            //}
        }

        private void OkButton_Click(object sender, RoutedEventArgs e)
        {
            SaveInstance();
        }

        private void SaveInstance()
        {
            _service.SaveInstanceAsync(_instance);
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            LoadCatalogAsync();
        }

        private void MachineCatalogComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var selectedName = MachineCatalogComboBox.SelectedItem as MachineDefinitionViewModel;
            string machineName = selectedName.MachineName;
            string newinstancename = machineName + "01";
            int counter = 1;

            bool nameResult = IsInstanceNameOk(machineName);
            do
            {
                newinstancename = $"{machineName}{counter:D2}"; // 01, 02, 03...
                counter++;
            }
            while (!IsInstanceNameOk(newinstancename));

            _instance.InstanceName = newinstancename;
            _instance.DisplayName = newinstancename;
        }
        private bool IsInstanceNameOk(string name)
        {
            bool resulut = true;
            var found = from n in _InstaneList where n.InstanceName == name select n;
            if (found.Any())
                resulut = false;
            return resulut;
        }

     
        private void _25button_Click(object sender, RoutedEventArgs e)
        {
            _instance.DefaultScale = .25;
        }

        private void _50button_Click(object sender, RoutedEventArgs e)
        {
            _instance.DefaultScale = .5;
        }

        private void _75button_Click(object sender, RoutedEventArgs e)
        {
            _instance.DefaultScale = .75;
        }

        private void _100button_Click(object sender, RoutedEventArgs e)
        {
            _instance.DefaultScale = 1;
        }

        private void _125button_Click(object sender, RoutedEventArgs e)
        {
            _instance.DefaultScale = 1.25;
        }

        private void _150button_Click(object sender, RoutedEventArgs e)
        {
            _instance.DefaultScale = 1.5;
        }

        private void _175button_Click(object sender, RoutedEventArgs e)
        {
            _instance.DefaultScale = 1.75;
        }

        private void _200button_Click(object sender, RoutedEventArgs e)
        {
            _instance.DefaultScale = 2;
        }

        private void _225button_Click(object sender, RoutedEventArgs e)
        {
            _instance.DefaultScale = 2.25;
        }

        private void _250button_Click(object sender, RoutedEventArgs e)
        {
            _instance.DefaultScale = 2.5;
        }

        private void _275button_Click(object sender, RoutedEventArgs e)
        {
            _instance.DefaultScale = 2.75;
        }

        private void _300button_Click(object sender, RoutedEventArgs e)
        {
            _instance.DefaultScale = 3;
        }

        private void _325button_Click(object sender, RoutedEventArgs e)
        {
            _instance.DefaultScale = 3.25;
        }

        private void _350button_Click(object sender, RoutedEventArgs e)
        {
            _instance.DefaultScale = 3.5;
        }

        private void _375button_Click(object sender, RoutedEventArgs e)
        {
            _instance.DefaultScale = 3.75;
        }

        private void _400button_Click(object sender, RoutedEventArgs e)
        {
            _instance.DefaultScale =4;
        }
    }
}
