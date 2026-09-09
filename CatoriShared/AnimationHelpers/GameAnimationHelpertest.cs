using CommunityToolkit.Mvvm.Messaging;
using CatoriApp.Core.Objects.Production;
using System.Globalization;
using CatoriApp.Game.Objects.AnimationOnPath;
using CatoriShared.AnimationHelpers;

namespace CatoriShared.AnimationHelpersTest;

/// <summary>
/// Generic WPF helpers for adding controls and paths to a Canvas and animating
/// controls along PathGeometry instances.
/// </summary>
public static class GameAnimationHelperTest
{
    public static PathGeometry CreatePathGeometry(
        IEnumerable<Point> points,
        bool isClosed = false,
        bool isFilled = false)
    {
        ArgumentNullException.ThrowIfNull(points);

        Point[] pointArray = points.ToArray();
        if (pointArray.Length < 2)
            throw new ArgumentException("A path requires at least two points.", nameof(points));

        var figure = new PathFigure
        {
            StartPoint = pointArray[0],
            IsClosed = isClosed,
            IsFilled = isFilled
        };

        figure.Segments.Add(new PolyLineSegment(pointArray.Skip(1), true));
        return new PathGeometry(new[] { figure });
    }

    public static PathGeometry ParsePathGeometry(string pathData)
    {
        Geometry geometry;
        PathGeometry resultPath = null;
        if (pathData != null && pathData != "" && pathData != "[]")
        {
            geometry = Geometry.Parse(pathData);
            resultPath = PathGeometry.CreateFromGeometry(geometry);
        }
        return resultPath;
    }
    public static PathAnimationHandleTest AddControlOnPath(string animationName,
        string partName, string targetName, string nextAction,
        Canvas canvas,FrameworkElement control,string itemDataJson,
        PathAnimationOptions? options = null)
    {
        Enum.TryParse<ProductionAction>(nextAction,true,out var action);
        return AddControlOnPathInner(
            animationName,
            partName,
            targetName,
            action,
            canvas,
            control,
            itemDataJson,
            options);
    }

    public static PathAnimationHandleTest AddControlOnPath(string animationName,
        string partName, string targetName, ProductionAction nextAction,
        Canvas canvas, FrameworkElement control, string itemDataJson,
        PathAnimationOptions? options = null)
    {
        return AddControlOnPathInner(animationName, partName, targetName, nextAction,
                canvas, control, itemDataJson, options);
    }

    public static PathAnimationHandleTest AddControlOnPath(PathAnimationDefinition definition,
        Canvas canvas,FrameworkElement control,PathAnimationOptions? options=null)
    {
        ArgumentNullException.ThrowIfNull(definition);
        return AddControlOnPath(definition.AnimationName,definition.PartName,definition.TargetName,
            definition.NextAction,
            canvas,control,definition.PathData,options);
    }

    public static PathAnimationHandleTest AddControlOnPath(string animationName,
        Canvas canvas, FrameworkElement control, string itemDataJson,
        PathAnimationOptions? options = null)
    {
        return AddControlOnPath(animationName, $"{animationName}Part", string.Empty, string.Empty,
                canvas, control, itemDataJson, options);
    }

    public static PathAnimationHandleTest AddControlOnPathInner(string animationName,
        string partName, string targetName, ProductionAction nextAction,
        Canvas canvas,
        FrameworkElement control,
        string itemDataJson,
        PathAnimationOptions? options = null)
    {
        ArgumentNullException.ThrowIfNull(canvas);
        ArgumentNullException.ThrowIfNull(control);
        ArgumentNullException.ThrowIfNull(itemDataJson);
        if (!ReferenceEquals(control.Parent, canvas))
            throw new InvalidOperationException(
                $"Animation '{animationName}' requires a control already attached directly to the supplied canvas.");

        string imagePath = control is CatoriInterfaces.IImagePathSource imageSource
            ? imageSource.ImagePath ?? string.Empty
            : string.Empty;
        System.Diagnostics.Debug.WriteLine(
            $"Animation '{animationName}' control '{control.GetType().FullName}' image path: " +
            $"{(string.IsNullOrWhiteSpace(imagePath) ? "<none>" : imagePath)}");
    
        LayoutItemPathSet pathSet = LayoutItemPathSetSerializer.Parse(itemDataJson);
        string initialPath = pathSet.Paths.OrderBy(path => path.Order)
            .FirstOrDefault(path => !string.IsNullOrWhiteSpace(path.WpfPath))?.WpfPath
            ?? throw new InvalidOperationException($"Animation '{animationName}' has no drawable paths.");
        PathGeometry geometry = ParsePathGeometry(initialPath);
        options ??= new PathAnimationOptions();
        EnsureNameScope(canvas);
        string _animationName = animationName;
        var scaleTransform = new ScaleTransform(1, 1);
        var rotateTransform = new RotateTransform();
        var translateTransform = new TranslateTransform();
        Transform originalTransform = control.RenderTransform;
        var transformGroup = new TransformGroup();
        if (originalTransform is Transform existingTransform
            && !ReferenceEquals(existingTransform, Transform.Identity))
            transformGroup.Children.Add(existingTransform);
        if (options.InitialScale > 0)
        {
            scaleTransform.ScaleX = options.InitialScale;
            scaleTransform.ScaleY = options.InitialScale;
            transformGroup.Children.Add(scaleTransform);
        }
        if (options.RotateWithPath)
            transformGroup.Children.Add(rotateTransform);

        transformGroup.Children.Add(translateTransform);
        
        // This helper animates an existing scene control in place. Its saved
        // layout size belongs to the scene and must not be replaced by path
        // animation sizing options.
        double width = ResolveDimension(control.Width, control.ActualWidth);
        double height = ResolveDimension(control.Height, control.ActualHeight);
        double baseLeft = NormalizeCanvasCoordinate(Canvas.GetLeft(control));
        double baseTop = NormalizeCanvasCoordinate(Canvas.GetTop(control));
        double centerX = options.ControlAnchorOnPath?.X * width
            ?? (options.CenterOnPath ? width / 2 : 0);
        double centerY = options.ControlAnchorOnPath?.Y * height
            ?? (options.CenterOnPath ? height / 2 : 0);
        double pathOffsetX = -baseLeft - centerX;
        double pathOffsetY = -baseTop - centerY;
        geometry.Transform = new TranslateTransform(pathOffsetX, pathOffsetY);

        Panel.SetZIndex(control, options.ZIndex);

        string suffix = Guid.NewGuid().ToString("N", CultureInfo.InvariantCulture);
        string translateName = $"PathTranslate_{suffix}";
        string? rotateName = options.RotateWithPath ? $"PathRotate_{suffix}" : null;

        canvas.RegisterName(translateName, translateTransform);
        if (rotateName != null)
            canvas.RegisterName(rotateName, rotateTransform);

        Storyboard storyboard = CreatePathStoryboard(
            geometry,
            translateName,
            rotateName,
            scaleTransform,
            options);

        var handle = new PathAnimationHandleTest(_animationName,
            partName,targetName,nextAction,
            canvas,
            control,
            transformGroup,
            originalTransform,
            storyboard,
            translateName,
            rotateName,pathSet,options,scaleTransform,
            pathOffsetX,pathOffsetY);

        if (options.AutoStart)
            handle.Start();

        return handle;
    }

    public static Storyboard CreatePathStoryboard(
        PathGeometry geometry,
        string translateTransformName,
        string? rotateTransformName,
        ScaleTransform scaleTransform,
        PathAnimationOptions options)
    {
        ArgumentNullException.ThrowIfNull(geometry);
        ArgumentException.ThrowIfNullOrWhiteSpace(translateTransformName);
        ArgumentNullException.ThrowIfNull(options);

        var storyboard = new Storyboard
        {
            FillBehavior = options.HoldAtEndUntilHandoff
                ? FillBehavior.HoldEnd
                : FillBehavior.Stop
        };
        if (options.InitialScale > 0)
        {
            var scaleXAnimation = new DoubleAnimation
            {
                From = options.InitialScale,
                To = 1.0,
                Duration = options.Duration
            };

            var scaleYAnimation = new DoubleAnimation
            {
                From = options.InitialScale,
                To = 1.0,
                Duration = options.Duration
            };

            Storyboard.SetTarget(scaleXAnimation, scaleTransform);
            Storyboard.SetTargetProperty(
                scaleXAnimation,
                new PropertyPath(ScaleTransform.ScaleXProperty));

            Storyboard.SetTarget(scaleYAnimation, scaleTransform);
            Storyboard.SetTargetProperty(
                scaleYAnimation,
                new PropertyPath(ScaleTransform.ScaleYProperty));

            storyboard.Children.Add(scaleXAnimation);
            storyboard.Children.Add(scaleYAnimation);
        }
        storyboard.Children.Add(CreatePathAnimation(
            geometry,
            PathAnimationSource.X,
            translateTransformName,
            TranslateTransform.XProperty,
            options.Duration,
            options.AccelerationRatio,
            options.DecelerationRatio));

        storyboard.Children.Add(CreatePathAnimation(
            geometry,
            PathAnimationSource.Y,
            translateTransformName,
            TranslateTransform.YProperty,
            options.Duration,
            options.AccelerationRatio,
            options.DecelerationRatio));

        if (options.RotateWithPath && !string.IsNullOrWhiteSpace(rotateTransformName))
        {
            storyboard.Children.Add(CreatePathAnimation(
                geometry,
                PathAnimationSource.Angle,
                rotateTransformName,
                RotateTransform.AngleProperty,
                options.Duration,
                options.AccelerationRatio,
                options.DecelerationRatio));
        }
     
        return storyboard;
    }

    public static void RemoveElement(Canvas canvas, UIElement? element)
    {
        if (canvas == null || element == null)
            return;

        canvas.Children.Remove(element);
    }

    public static void SetDesignModeStyle(
        Path path,
        bool isDesignMode,
        Brush? designStroke = null,
        Brush? playStroke = null)
    {
        ArgumentNullException.ThrowIfNull(path);

        if (isDesignMode)
        {
            path.Stroke = designStroke ?? Brushes.LimeGreen;
            path.StrokeThickness = 4;
            path.Opacity = 1.0;
        }
        else
        {
            path.Stroke = playStroke ?? Brushes.LightGreen;
            path.StrokeThickness = 2;
            path.Opacity = 0.12;
        }
    }

    private static DoubleAnimationUsingPath CreatePathAnimation(
        PathGeometry geometry,
        PathAnimationSource source,
        string targetName,
        DependencyProperty targetProperty,
        TimeSpan duration,
        double accelerationRatio,
        double decelerationRatio)
    {
        var animation = new DoubleAnimationUsingPath
        {
            PathGeometry = geometry,
            Source = source,
            Duration = duration,
            AccelerationRatio = accelerationRatio,
            DecelerationRatio = decelerationRatio
        };

        Storyboard.SetTargetName(animation, targetName);
        Storyboard.SetTargetProperty(animation, new PropertyPath(targetProperty));
        return animation;
    }

    private static double ResolveDimension(double requested, double actual)
    {
        if (!double.IsNaN(requested) && requested > 0)
            return requested;

        return actual > 0 ? actual : 0;
    }

    private static double NormalizeCanvasCoordinate(double value)
    {
        return double.IsNaN(value) ? 0 : value;
    }

    private static void EnsureNameScope(FrameworkElement element)
    {
        if (NameScope.GetNameScope(element) == null)
            NameScope.SetNameScope(element, new NameScope());
    }

}


public sealed class PathAnimationHandleTest : IDisposable
{
    private readonly Canvas _canvas;
    private readonly string _translateName;
    private readonly string? _rotateName;
    private bool _isStarted;
    private bool _isDisposed;
    private bool _isStartPending;
    private CancellationTokenSource? _startDelayCancellation;
    private string _animationName;
    private string PartName;
    private string TargetName;
    private ProductionAction NextAction;
    private readonly string _defaultPartName;
    private readonly LayoutItemPathSet _pathSet;
    private readonly PathAnimationOptions _options;
    private readonly ScaleTransform _scaleTransform;
    private readonly bool _animatesExistingControl;
    private readonly double _pathOffsetX;
    private readonly double _pathOffsetY;
    private readonly TransformGroup _animationTransform;
    private readonly Transform _originalTransform;
    private int _sequentialIndex;
    internal PathAnimationHandleTest(string animationName,
        string partName, string targetName, ProductionAction nextAction,
        Canvas canvas,
        FrameworkElement animationControl,
        TransformGroup animationTransform,
        Transform originalTransform,
        Storyboard storyboard,
        string translateName,
        string? rotateName,
        LayoutItemPathSet pathSet,
        PathAnimationOptions options,
        ScaleTransform scaleTransform,
        double pathOffsetX,
        double pathOffsetY)
    {
        _canvas = canvas;
        _animationName = animationName; 
        PartName = partName;
        _defaultPartName = partName;
        TargetName = targetName;
        NextAction = nextAction;
        _pathSet = pathSet;
        _options = options;
        _scaleTransform = scaleTransform;
        _path = pathSet.Paths.OrderBy(candidate => candidate.Order).First().WpfPath;
        AnimationControl = animationControl;
        _animationTransform = animationTransform;
        _originalTransform = originalTransform;
        _animatesExistingControl = true;
        _pathOffsetX = pathOffsetX;
        _pathOffsetY = pathOffsetY;
        Storyboard = storyboard;
        _translateName = translateName;
        _rotateName = rotateName;
        storyboard.Completed += (s, e) => StoryBoardComplete();
        WeakReferenceMessenger.Default.Register<RobotPartTransferMessage>(this,
            static (recipient,message)=>((PathAnimationHandleTest)recipient).HandlePartTransfer(message));
    }

    private void StoryBoardComplete()
    {
        try
        {
            _options.Completed?.Invoke(AnimationControl);
            AnimationCompleteMessage animationComplete =
                new AnimationCompleteMessage(_animationName,PartName,TargetName,NextAction,
                    AnimationControl);
            WeakReferenceMessenger.Default.Send<AnimationCompleteMessage>(animationComplete);
        }
        catch(Exception ex)
        {
            CatoriShared.Diagnostics.GameSafetyLog.Error("Animation",
                $"Completion handling failed for animation '{_animationName}'.",ex);
            RestoreSaneState();
        }

        //StopGlow();
    }

    private void HandlePartTransfer(RobotPartTransferMessage message)
    {
        if(!string.Equals(message.RobotName,TargetName,StringComparison.Ordinal))
            return;
        bool isPickup=NextAction==ProductionAction.Pickup;
        bool isDrop=NextAction==ProductionAction.Drop;
        bool shouldHide=message.Stage==RobotPartTransferStage.PickedUp&&(isPickup||isDrop)
            ||message.Stage==RobotPartTransferStage.Dropped&&isDrop;
        if(!shouldHide)return;
        void HidePlaceholder()
        {
            AnimationControl.Visibility=Visibility.Collapsed;
            Storyboard.Stop(_canvas);
        }
        if(AnimationControl.Dispatcher.CheckAccess())HidePlaceholder();
        else AnimationControl.Dispatcher.BeginInvoke((Action)HidePlaceholder);
    }

    private string _path;

    public FrameworkElement AnimationControl { get; }
    public string AnimationName=>_animationName;
    public Storyboard Storyboard { get; private set; }
  
           
    public async void Start()
    {
        ThrowIfDisposed();
        if(_isStartPending||_isStarted)return;
        try
        {
            if(_options.StartDelay>TimeSpan.Zero)
            {
                _isStartPending=true;
                AnimationControl.Visibility=Visibility.Collapsed;
                _startDelayCancellation?.Dispose();
                _startDelayCancellation=new CancellationTokenSource();
                await Task.Delay(_options.StartDelay,_startDelayCancellation.Token);
                _isStartPending=false;
                if(_isDisposed)return;
            }
            SelectPathForStart();
            AnimationControl.RenderTransformOrigin = _options.RenderTransformOrigin;
            AnimationControl.RenderTransform = _animationTransform;
            AnimationControl.Visibility=Visibility.Visible;
            _options.Started?.Invoke(AnimationControl);
            Storyboard.Begin(_canvas, HandoffBehavior.SnapshotAndReplace, true);
            _isStarted = true;
        }
        catch(OperationCanceledException)
        {
            _isStartPending=false;
        }
        catch(Exception ex)
        {
            _isStartPending=false;
            CatoriShared.Diagnostics.GameSafetyLog.Error("Animation",
                $"Animation '{_animationName}' failed to start and was reset.",ex);
            RestoreSaneState();
        }
    }

    private void RestoreSaneState()
    {
        try { Storyboard.Stop(_canvas); } catch(Exception stopFailure)
        {
            CatoriShared.Diagnostics.GameSafetyLog.Error("Animation",
                $"Animation '{_animationName}' also failed while stopping.",stopFailure);
        }
        if(_canvas.FindName(_translateName) is TranslateTransform translation)
        {translation.X=0;translation.Y=0;}
        if(_rotateName!=null&&_canvas.FindName(_rotateName) is RotateTransform rotation)
            rotation.Angle=0;
        _scaleTransform.ScaleX=1;
        _scaleTransform.ScaleY=1;
        AnimationControl.Visibility=Visibility.Visible;
        _isStarted=false;
    }

    private void SelectPathForStart()
    {
        var paths = _pathSet.Paths.Where(path => !string.IsNullOrWhiteSpace(path.WpfPath))
            .OrderBy(path => path.Order).ToList();
        if (paths.Count == 0) return;
        int index = _pathSet.SelectionMode switch
        {
            PathSelectionMode.Random => Random.Shared.Next(paths.Count),
            PathSelectionMode.Sequential => _sequentialIndex++ % paths.Count,
            _ => 0
        };
        LayoutPath selected = paths[index];
        _path = selected.WpfPath;
        PartName = string.IsNullOrWhiteSpace(selected.PartName) ? _defaultPartName : selected.PartName;
        Storyboard.Stop(_canvas);
        PathGeometry geometry = GameAnimationHelperTest.ParsePathGeometry(_path);
        if (_animatesExistingControl)
            geometry.Transform = new TranslateTransform(_pathOffsetX,_pathOffsetY);
        Storyboard = GameAnimationHelperTest.CreatePathStoryboard(
            geometry, _translateName, _rotateName, _scaleTransform, _options);
        Storyboard.Completed += (s, e) => StoryBoardComplete();
    }

    public void StartWithPart(FrameworkElement part,string? partName=null)
    {
        ThrowIfDisposed();
        ArgumentNullException.ThrowIfNull(part);
        if (!ReferenceEquals(part,AnimationControl))
            throw new InvalidOperationException(
                "Direct animation handles cannot replace their animation target during a path handoff.");
        if(!string.IsNullOrWhiteSpace(partName))PartName=partName;
        Start();
    }

    internal FrameworkElement? ReleasePartForPathHandoff()
    {
        ThrowIfDisposed();
        Storyboard.Stop(_canvas);
        _isStarted=false;
        return AnimationControl;
    }

    public void Pause()
    {
        ThrowIfDisposed();
        if (_isStarted)
            Storyboard.Pause(_canvas);
    }

    public void Resume()
    {
        ThrowIfDisposed();
        if (_isStarted)
            Storyboard.Resume(_canvas);
    }

    public void Stop()
    {
        ThrowIfDisposed();
        _startDelayCancellation?.Cancel();
        _isStartPending=false;
        if (_isStarted)
        {
            Storyboard.Stop(_canvas);
            _isStarted = false;
        }
    }

    public void Remove()
    {
        if (_isDisposed)
            return;

        _startDelayCancellation?.Cancel();
        _isStartPending=false;

        if (_isStarted)
        {
            Storyboard.Remove(_canvas);
            _isStarted = false;
        }

        if (ReferenceEquals(AnimationControl.RenderTransform,_animationTransform))
            AnimationControl.RenderTransform=_originalTransform;
        TryUnregisterName(_translateName);

        if (_rotateName != null)
            TryUnregisterName(_rotateName);
    }

    public void Dispose()
    {
        if (_isDisposed)
            return;

        Remove();
        _startDelayCancellation?.Dispose();
        WeakReferenceMessenger.Default.UnregisterAll(this);
        _isDisposed = true;
    }

    private void TryUnregisterName(string name)
    {
        try
        {
            _canvas.UnregisterName(name);
        }
        catch (ArgumentException)
        {
            // The name may already have been removed by the owning _view.
        }
    }

    private void ThrowIfDisposed()
    {
        ObjectDisposedException.ThrowIf(_isDisposed, this);
    }

}
