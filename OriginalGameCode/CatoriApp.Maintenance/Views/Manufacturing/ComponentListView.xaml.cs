namespace CatoriApp.Game.Views.Manufacturing
{
    public partial class ComponentListView : Window
    {
        private readonly ProductBuilderViewController _controller = new();
        private ObservableCollection<ComponentViewModel> _components = new();

        public ComponentListView()
        {
            InitializeComponent();
        }

        private async void Window_Loaded(object sender, RoutedEventArgs e)
        {
            await LoadComponentsAsync();
        }

        private async Task LoadComponentsAsync()
        {
            await _controller.LoadComponents();
            _components = _controller.componentVM;
            ComponentsDataGrid.ItemsSource = _components;
            StatusTextBlock.Text = $"Loaded {_components.Count} components.";
        }

        private ComponentViewModel? SelectedComponent => ComponentsDataGrid.SelectedItem as ComponentViewModel;

        private void NewButton_Click(object sender, RoutedEventArgs e)
        {
            var component = new ComponentViewModel
            {
                ComponentName = "New Component",
                Quantity = 0
            };

            _components.Add(component);
            ComponentsDataGrid.SelectedItem = component;
            EditComponent(component);
        }

        private void EditButton_Click(object sender, RoutedEventArgs e)
        {
            if (SelectedComponent != null)
                EditComponent(SelectedComponent);
        }

        private async void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            if (SelectedComponent == null)
                return;

            await SaveComponentAsync(SelectedComponent);
        }

        private async void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            if (SelectedComponent == null)
                return;

            var component = SelectedComponent;
            await _controller.RemoveComponentAsync(component);
            _components.Remove(component);
            StatusTextBlock.Text = "Component deleted.";
        }

        private void DataGridRow_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (sender is DataGridRow row && row.Item is ComponentViewModel component)
                EditComponent(component);
        }

        private void EditComponent(ComponentViewModel component)
        {
            var view = new ComponentEditorView(component)
            {
                Owner = this
            };

            if (view.ShowDialog() == true)
            {
                ComponentsDataGrid.Items.Refresh();
                StatusTextBlock.Text = "Component saved.";
            }
        }

        private async Task SaveComponentAsync(ComponentViewModel component)
        {
            await _controller.UpdateComponentAsync(component);
            ComponentsDataGrid.Items.Refresh();
            StatusTextBlock.Text = "Component saved.";
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}