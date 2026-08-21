using CatoriApp.Game.Objects.AnimationOnPath;
using CommunityToolkit.Mvvm.Messaging;
using CatoriUCLibrary.Views.FactoryControls;

namespace CatoriApp.Game.Controllers
{
    public abstract class ControllerAnimateLayoutsBase : IDisposable
    {
        public LocationLayoutItemService _layoutItemService { get; } = new();
        public long _locationId { get; protected set; }
        protected readonly PathAnimationSession _productionSession = new();
        protected IReadOnlyList<LocationLayoutItemViewModel> LayoutItems { get; private set; }
            = Array.Empty<LocationLayoutItemViewModel>();

        protected ControllerAnimateLayoutsBase(long locationId = 0)
        {
            _locationId = locationId;
            WeakReferenceMessenger.Default.Register<AnimationCompleteMessage>(
                this, static (recipient, message) =>
                    ((ControllerAnimateLayoutsBase)recipient).OnAnimationCompleted(message));
            WeakReferenceMessenger.Default.Register<FactoryControlPanelActivatedMessage>(
                this, static (recipient, message) =>
                    ((ControllerAnimateLayoutsBase)recipient).OnFactoryControlPanelActivated(message));
        }

        protected async Task<IReadOnlyList<LocationLayoutItemViewModel>> LoadLayoutItemsAsync()
        {
            if (_locationId <= 0)
            {
                LayoutItems = Array.Empty<LocationLayoutItemViewModel>();
                return LayoutItems;
            }

            LayoutItems = await _layoutItemService.GetByLocationIdAsync(_locationId);
            return LayoutItems;
        }

        protected IReadOnlyList<LocationLayoutItemViewModel> LoadLayoutItemsNow()
            => LoadLayoutItemsAsync().GetAwaiter().GetResult();

        protected virtual void OnAnimationCompleted(AnimationCompleteMessage message)
        {
        }

        protected virtual void OnFactoryControlPanelActivated(FactoryControlPanelActivatedMessage message)
        {
            if (!string.IsNullOrWhiteSpace(message.AnimationTargetName))
                _productionSession.StartPath(message.AnimationTargetName);
        }

        public virtual void Dispose()
        {
            _productionSession.Dispose();
            WeakReferenceMessenger.Default.UnregisterAll(this);
            GC.SuppressFinalize(this);
        }
    }
}
