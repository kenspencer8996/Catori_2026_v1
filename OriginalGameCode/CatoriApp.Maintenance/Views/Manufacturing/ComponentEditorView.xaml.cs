namespace CatoriApp.Game.Views.Manufacturing
{
    public partial class ComponentEditorView : Window
    {
        private readonly ProductBuilderViewController _controller = new();
        private readonly ComponentViewModel _component;

        public ComponentEditorView(ComponentViewModel component)
        {
            _component = component;
            InitializeComponent();
            DataContext = _component;
        }

        private async void OkButton_Click(object sender, RoutedEventArgs e)
        {
            await _controller.UpdateComponentAsync(_component);
            DialogResult = true;
            Close();
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void ExitButton_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}