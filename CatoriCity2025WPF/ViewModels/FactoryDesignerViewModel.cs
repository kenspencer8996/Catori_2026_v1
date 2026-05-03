using System.Collections.ObjectModel;

namespace CatoriCity2025WPF.ViewModels
{
    public class FactoryDesignerViewModel : ViewmodelBase
    {
        private long _factoryId;
        private long _factoryLayoutId;
        private string _factoryName = "";
        private string _layoutName = "";
        private string? _backgroundImagePath;
        private double _canvasWidth = 1920;
        private double _canvasHeight = 1080;
        private FactoryLayoutItemViewModel? _selectedItem;

        public long FactoryId { get => _factoryId; set => SetProperty(ref _factoryId, value); }
        public long FactoryLayoutId { get => _factoryLayoutId; set => SetProperty(ref _factoryLayoutId, value); }
        public string FactoryName { get => _factoryName; set => SetProperty(ref _factoryName, value); }
        public string LayoutName { get => _layoutName; set => SetProperty(ref _layoutName, value); }
        public string? BackgroundImagePath { get => _backgroundImagePath; set => SetProperty(ref _backgroundImagePath, value); }
        public double CanvasWidth { get => _canvasWidth; set => SetProperty(ref _canvasWidth, value); }
        public double CanvasHeight { get => _canvasHeight; set => SetProperty(ref _canvasHeight, value); }
        public FactoryLayoutItemViewModel? SelectedItem { get => _selectedItem; set => SetProperty(ref _selectedItem, value); }
        public ObservableCollection<FactoryLayoutItemViewModel> Items { get; } = new();
        public ObservableCollection<FactoryPartRouteViewModel> Routes { get; } = new();
    }
}
