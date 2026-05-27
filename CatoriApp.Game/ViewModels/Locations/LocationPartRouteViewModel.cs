using System.Collections.ObjectModel;
namespace CatoriApp.Game.ViewModels.Locations
{
    public class LocationPartRouteViewModel : ViewmodelBase
    {
        private long _locationPartRouteId;
        private long _locationLayoutId;
        private string _routeName = "";
        private int? _productId;
        private long? _fromItemId;
        private long? _toItemId;
        private bool _isActive = true;
        private DateTime _createdAt = DateTime.Now;

        public long LocationPartRouteId { get => _locationPartRouteId; set => SetProperty(ref _locationPartRouteId, value); }
        public long LocationId { get => _locationLayoutId; set => SetProperty(ref _locationLayoutId, value); }
        public string RouteName { get => _routeName; set => SetProperty(ref _routeName, value); }
        public int? ProductId { get => _productId; set => SetProperty(ref _productId, value); }
        public long? FromItemId { get => _fromItemId; set => SetProperty(ref _fromItemId, value); }
        public long? ToItemId { get => _toItemId; set => SetProperty(ref _toItemId, value); }
        public bool IsActive { get => _isActive; set => SetProperty(ref _isActive, value); }
        public DateTime CreatedAt { get => _createdAt; set => SetProperty(ref _createdAt, value); }
        public ObservableCollection<LocationPartRoutePointViewModel> Points { get; } = new();
    }
}


