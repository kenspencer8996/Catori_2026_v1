using CatoriApp.Game.Objects.AnimationOnPath;
using CommunityToolkit.Mvvm.Messaging;
using CatoriUCLibrary.Views.FactoryControls;
using CatoriShared.Diagnostics;
using System.Windows.Media.Animation;
using System.Reflection;

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

            try
            {
                LayoutItems = await _layoutItemService.GetByLocationIdAsync(_locationId);
            }
            catch (Exception ex)
            {
                GameSafetyLog.Error("Location loading",
                    $"Location {_locationId} could not load its layout. Showing an empty safe layout.", ex);
                LayoutItems = Array.Empty<LocationLayoutItemViewModel>();
            }
            return LayoutItems;
        }

        protected IReadOnlyList<LocationLayoutItemViewModel> LoadLayoutItemsNow()
            => LoadLayoutItemsAsync().GetAwaiter().GetResult();

        protected virtual void OnAnimationCompleted(AnimationCompleteMessage message)
        {
            if(message.Action==CatoriApp.Core.Objects.Production.ProductionAction.TransitionToView)
                _=TransitionToLocationViewAsync(message.TargetName);
        }

        protected async Task TransitionToLocationViewAsync(string? locationName)
        {
            if(string.IsNullOrWhiteSpace(locationName))return;
            string expectedName=new string(locationName.Where(char.IsLetterOrDigit).ToArray())+"View";
            Type? viewType=AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(assembly=>
                {
                    try{return assembly.GetTypes();}
                    catch(ReflectionTypeLoadException ex){return ex.Types.Where(type=>type!=null)!;}
                })
                .FirstOrDefault(type=>type!=null&&typeof(Window).IsAssignableFrom(type)
                    &&(string.Equals(type.Name,expectedName,StringComparison.OrdinalIgnoreCase)
                        ||type.Namespace?.Contains($".{locationName}.",StringComparison.OrdinalIgnoreCase)==true));
            if(viewType==null||Activator.CreateInstance(viewType) is not Window target)
            {
                GameSafetyLog.Warning("View transition",$"No Window view was found for location '{locationName}'.");
                return;
            }
            Window? source=Application.Current.Windows.OfType<Window>().FirstOrDefault(window=>window.IsActive)
                ??Application.Current.MainWindow;
            if(source!=null)await FadeAsync(source,source.Opacity,0,TimeSpan.FromMilliseconds(450));
            target.Opacity=0;
            target.Show();
            await FadeAsync(target,0,1,TimeSpan.FromMilliseconds(450));
            if(source!=null&&source!=target)source.Hide();
        }

        private static Task FadeAsync(Window window,double from,double to,TimeSpan duration)
        {
            var completion=new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
            var animation=new DoubleAnimation(from,to,new Duration(duration));
            animation.Completed+=(_,_)=>completion.TrySetResult();
            window.BeginAnimation(UIElement.OpacityProperty,animation);
            return completion.Task;
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
