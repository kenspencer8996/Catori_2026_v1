using System.Collections.ObjectModel;

namespace CatoriCity2025WPF.ViewModels
{
    public class FactoryPartRouteViewModel : ViewmodelBase
    {
        private long _factoryPartRouteId;
        private long _factoryLayoutId;
        private string _routeName = "";
        private int? _productId;
        private long? _fromItemId;
        private long? _toItemId;
        private bool _isActive = true;
        private DateTime _createdAt = DateTime.Now;

        public long FactoryPartRouteId { get => _factoryPartRouteId; set => SetProperty(ref _factoryPartRouteId, value); }
        public long FactoryLayoutId { get => _factoryLayoutId; set => SetProperty(ref _factoryLayoutId, value); }
        public string RouteName { get => _routeName; set => SetProperty(ref _routeName, value); }
        public int? ProductId { get => _productId; set => SetProperty(ref _productId, value); }
        public long? FromItemId { get => _fromItemId; set => SetProperty(ref _fromItemId, value); }
        public long? ToItemId { get => _toItemId; set => SetProperty(ref _toItemId, value); }
        public bool IsActive { get => _isActive; set => SetProperty(ref _isActive, value); }
        public DateTime CreatedAt { get => _createdAt; set => SetProperty(ref _createdAt, value); }
        public ObservableCollection<FactoryPartRoutePointViewModel> Points { get; } = new();
    }
}
