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
        private static readonly SemaphoreSlim LocationTransitionLock = new(1, 1);

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
                {
                    var controller = (ControllerAnimateLayoutsBase)recipient;
                    if (controller.OwnsAnimation(message))
                        controller.OnAnimationCompleted(message);
                });
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
        {
            return LoadLayoutItemsAsync().GetAwaiter().GetResult();
        }

        protected virtual void OnAnimationCompleted(AnimationCompleteMessage message)
        {
            if(message.Action==CatoriApp.Core.Objects.Production.ProductionAction.TransitionToView)
                _=TransitionToLocationViewAsync(message.TargetName);
        }

        // Animation completion is broadcast application-wide. Controllers must
        // opt in only for animations owned by their scene.
        protected virtual bool OwnsAnimation(AnimationCompleteMessage message) => false;

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
            if(viewType==null)
            {
                GameSafetyLog.Warning("View transition",$"No Window view was found for location '{locationName}'.");
                return;
            }

            await LocationTransitionLock.WaitAsync();
            try
            {
                Window? source=Application.Current.Windows.OfType<Window>().FirstOrDefault(window=>window.IsActive)
                    ??Application.Current.MainWindow;
                Window? target=Application.Current.Windows.OfType<Window>()
                    .FirstOrDefault(window=>window.GetType()==viewType);

                if(ReferenceEquals(source,target))return;

                bool createdTarget=target==null;
                if(target==null&&Activator.CreateInstance(viewType) is Window created)
                    target=created;
                if(target==null)
                {
                    GameSafetyLog.Warning("View transition",$"Window view '{viewType.FullName}' could not be created.");
                    return;
                }

                if(createdTarget&&source!=null)
                {
                    Window previous=source;
                    target.Closed+=(_,_)=>
                    {
                        if(previous.IsLoaded)
                        {
                            previous.Opacity=1;
                            previous.Show();
                            previous.Activate();
                        }
                    };
                }

                if(source!=null)await FadeAsync(source,source.Opacity,0,TimeSpan.FromMilliseconds(450));
                target.Opacity=0;
                if(!target.IsVisible)target.Show();
                target.Activate();
                await FadeAsync(target,0,1,TimeSpan.FromMilliseconds(450));
                if(source!=null&&source!=target)source.Hide();
            }
            finally
            {
                LocationTransitionLock.Release();
            }
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
