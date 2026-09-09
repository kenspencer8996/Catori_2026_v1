using CatoriApp.Maintenance.Objects.Services.Production;
using CatoriApp.Maintenance.ViewModels.Production;

namespace CatoriApp.Maintenance.Views.Production
{
    public partial class FactoryCapabilityEditorView : Window
    {
        private readonly ProductionMaintenanceService _service = new();
        private readonly FactoryCapabilityViewModel _model;

        public FactoryCapabilityEditorView(FactoryCapabilityViewModel model)
        {
            _model = model;
            InitializeComponent();
            DataContext = _model;
        }

        private async void OkButton_Click(object sender, RoutedEventArgs e)
        {
            await _service.SaveFactoryCapabilityAsync(_model);
            DialogResult = true;
            Close();
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}