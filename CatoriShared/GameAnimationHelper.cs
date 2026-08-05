using CommunityToolkit.Mvvm.Messaging;
using System.Globalization;

namespace CatoriApp.Game.Objects.AnimationOnPath;

/// <summary>
/// Generic WPF helpers for adding controls and paths to a Canvas and animating
/// controls along PathGeometry instances.
/// </summary>
public static class GameAnimationHelper
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
        if (string.IsNullOrWhiteSpace(pathData))
            throw new ArgumentException("Path data cannot be empty.", nameof(pathData));

        Geometry geometry = Geometry.Parse(pathData);
        return PathGeometry.CreateFromGeometry(geometry);
    }
    public static PathAnimationHandle AddControlOnPath(string animationName,
    Canvas canvas,FrameworkElement control,string itemDataJson,
    PathAnimationOptions? options = null)
    {
        return AddControlOnPathInner(
            animationName,
            canvas,
            control,
            itemDataJson,
            options);
    }
    public static Path AddPath(
    Canvas canvas,
    string itemDataJson,
    PathDisplayOptions? options = null)
    {
       
        
        return AddPath(
            canvas,
            ParsePathGeometry(itemDataJson),
            options);
    }
    public static Path AddPath(
        Canvas canvas,
        Geometry geometry,
        PathDisplayOptions? options = null)
    {
        ArgumentNullException.ThrowIfNull(canvas);
        ArgumentNullException.ThrowIfNull(geometry);

        options ??= new PathDisplayOptions();

        var path = new Path
        {
            Data = geometry,
            Stroke = options.Stroke,
            StrokeThickness = options.StrokeThickness,
            StrokeDashArray = options.DashArray,
            StrokeStartLineCap = options.LineCap,
            StrokeEndLineCap = options.LineCap,
            StrokeLineJoin = options.LineJoin,
            Fill = options.Fill,
            Opacity = options.Opacity,
            IsHitTestVisible = options.IsHitTestVisible,
            Tag = options.Tag
        };

        Panel.SetZIndex(path, options.ZIndex);
        canvas.Children.Add(path);
        return path;
    }

    public static ContentControl AddControl(
        Canvas canvas,
        FrameworkElement control,
        Point location,
        CanvasControlOptions? options = null)
    {
        ArgumentNullException.ThrowIfNull(canvas);
        ArgumentNullException.ThrowIfNull(control);

        options ??= new CanvasControlOptions();

        var host = new ContentControl
        {
            Content = control,
            IsHitTestVisible = options.IsHitTestVisible,
            Opacity = options.Opacity,
            Tag = options.Tag
        };

        double width = ResolveDimension(control.Width, control.ActualWidth);
        double height = ResolveDimension(control.Height, control.ActualHeight);

        double left = options.CenterOnLocation ? location.X - width / 2 : location.X;
        double top = options.CenterOnLocation ? location.Y - height / 2 : location.Y;

        Canvas.SetLeft(host, left);
        Canvas.SetTop(host, top);
        Panel.SetZIndex(host, options.ZIndex);
        canvas.Children.Add(host);

        return host;
    }

    public static PathAnimationHandle AddControlOnPathInner(string animationName,
        Canvas canvas,
        FrameworkElement control,
        string itemDataJson,
        PathAnimationOptions? options = null)
    {
        ArgumentNullException.ThrowIfNull(canvas);
        ArgumentNullException.ThrowIfNull(control);
        ArgumentNullException.ThrowIfNull(itemDataJson);
    
        PathGeometry geometry = ParsePathGeometry(itemDataJson);
        options ??= new PathAnimationOptions();
        EnsureNameScope(canvas);
        string _animationName = animationName;
        var scaleTransform = new ScaleTransform(1, 1);
        var rotateTransform = new RotateTransform();
        var translateTransform = new TranslateTransform();
        var transformGroup = new TransformGroup();
        if (options.InitialScale > 0)
        {
            scaleTransform.ScaleX = options.InitialScale;
            scaleTransform.ScaleY = options.InitialScale;
            transformGroup.Children.Add(scaleTransform);
        }
        if (options.RotateWithPath)
            transformGroup.Children.Add(rotateTransform);

        transformGroup.Children.Add(translateTransform);

        Point startPoint = geometry.Figures.Count > 0
            ? geometry.Figures[0].StartPoint
            : new Point();

        translateTransform.X = startPoint.X;
        translateTransform.Y = startPoint.Y;

        var host = new ContentControl
        {
            Content = control,
            RenderTransform = transformGroup,
            RenderTransformOrigin = options.RenderTransformOrigin,
            IsHitTestVisible = options.IsHitTestVisible,
            Opacity = options.Opacity,
            Tag = options.Tag
        };

        double width = options.ControlWidth ?? ResolveDimension(control.Width, control.ActualWidth);
        double height = options.ControlHeight ?? ResolveDimension(control.Height, control.ActualHeight);

        Canvas.SetLeft(host, options.CenterOnPath ? -width / 2 : 0);
        Canvas.SetTop(host, options.CenterOnPath ? -height / 2 : 0);
        Panel.SetZIndex(host, options.ZIndex);

        if (!canvas.Children.Contains(host))
        {
            canvas.Children.Add(host);
        }
        //canvas.Children.Add(host);

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

        var handle = new PathAnimationHandle(_animationName,
            canvas,
            host,
            storyboard,
            translateName,
            rotateName,itemDataJson);

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

        var storyboard = new Storyboard();
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

    private static void EnsureNameScope(FrameworkElement element)
    {
        if (NameScope.GetNameScope(element) == null)
            NameScope.SetNameScope(element, new NameScope());
    }

    private static Path CreatePath(
    Canvas canvas,
    string itemDataJson,
    Brush stroke,
    double thickness = 3)
    {
        var path = new Path
        {
            Data = Geometry.Parse(itemDataJson),
            Stroke = stroke,
            StrokeThickness = thickness,
            Fill = Brushes.Transparent,
            IsHitTestVisible = false,
            Opacity = 0.6,
            Effect = new DropShadowEffect
            {
                Color = Colors.DeepSkyBlue,
                ShadowDepth = 0,
                BlurRadius = 8,
                Opacity = 0.35
            }
        };

        canvas.Children.Add(path);

        return path;
    }
}

public sealed class PathDisplayOptions
{
    public Brush Stroke { get; init; } = Brushes.LimeGreen;
    public Brush? Fill { get; init; } = Brushes.Transparent;
    public double StrokeThickness { get; init; } = 4;
    public double Opacity { get; init; } = 1;
    public DoubleCollection? DashArray { get; init; }
    public PenLineCap LineCap { get; init; } = PenLineCap.Round;
    public PenLineJoin LineJoin { get; init; } = PenLineJoin.Round;
    public bool IsHitTestVisible { get; init; }
    public int ZIndex { get; init; } = 2000;
    public object? Tag { get; init; }
}

public sealed class CanvasControlOptions
{
    public bool CenterOnLocation { get; init; } = true;
    public bool IsHitTestVisible { get; init; } = true;
    public double Opacity { get; init; } = 1;
    public int ZIndex { get; init; } = 2001;
    public object? Tag { get; init; }
}

public sealed class PathAnimationOptions
{
    public TimeSpan Duration { get; set; } = TimeSpan.FromSeconds(5);
    public RepeatBehavior RepeatBehavior { get; set; } = new RepeatBehavior(1);
    public bool AutoReverse { get; set; }
    public bool AutoStart { get; set; } = true;
    public bool RotateWithPath { get; set; } = true;
    public bool CenterOnPath { get; set; } = true;
    public bool IsHitTestVisible { get; set; }
    public double Opacity { get; set; } = 1;
    public double SpeedRatio { get; set; } = 1;
    public double AccelerationRatio { get; set; }
    public double DecelerationRatio { get; set; }
    public double? ControlWidth { get; set; }
    public double? ControlHeight { get; set; }
    public int ZIndex { get; set; } = 2002;
    public Point RenderTransformOrigin { get; set; } = new(0.5, 0.5);
    public object? Tag { get; set; }
    /// <summary>
    /// Initial scale of the control.
    /// 1.0 = normal size.
    /// 0 = disabled.
    /// 0.25 = starts at 25% and grows to 100%.
    /// </summary>
    public double InitialScale { get; set; } = 0;
}

public sealed class PathAnimationHandle : IDisposable
{
    private readonly Canvas _canvas;
    private readonly string _translateName;
    private readonly string? _rotateName;
    private bool _isStarted;
    private bool _isDisposed;
    private string _animationName;

    internal PathAnimationHandle(string animationName,
        Canvas canvas,
        ContentControl host,
        Storyboard storyboard,
        string translateName,
        string? rotateName,
        string path)
    {
        _canvas = canvas;
        _animationName = animationName; 
        _path = path;
        Host = host;
        Storyboard = storyboard;
        _translateName = translateName;
        _rotateName = rotateName;
        storyboard.Completed += (s, e) => StoryBoardComplete();
        _visualPath = new Path
        {
            Data = Geometry.Parse(_path),
            Stroke = Brushes.DeepSkyBlue,
            StrokeThickness = 3,
            Opacity = 0.65,
            IsHitTestVisible = false,
            Effect = new DropShadowEffect
            {
                Color = Colors.DeepSkyBlue,
                ShadowDepth = 0,
                BlurRadius = 6,
                Opacity = 0.2
            }
        };
        if (!canvas.Children.Contains(_visualPath))
        {
            canvas.Children.Add(_visualPath);
        }
    }

    private void StoryBoardComplete()
    {
        AnimationCompleteMessage animationComplete = new AnimationCompleteMessage(_animationName);
        WeakReferenceMessenger.Default.Send<AnimationCompleteMessage>(animationComplete );

        //StopGlow();
    }

    private string _path;

    public ContentControl Host { get; }
    public Storyboard Storyboard { get; }
  
    private readonly Path _visualPath;
    private Storyboard? _glowStoryboard;
           
    public void Start()
    {
        ThrowIfDisposed();
        StartGlow();
        Storyboard.Begin(_canvas, HandoffBehavior.SnapshotAndReplace, true);
        _isStarted = true;
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

        if (_isStarted)
        {
            Storyboard.Remove(_canvas);
            _isStarted = false;
        }

        _canvas.Children.Remove(Host);
        TryUnregisterName(_translateName);

        if (_rotateName != null)
            TryUnregisterName(_rotateName);
    }

    public void Dispose()
    {
        if (_isDisposed)
            return;

        Remove();
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

    private void StartGlow()
    {
        if (_glowStoryboard != null)
            return;

        if (_visualPath.Effect is not DropShadowEffect glow)
            return;

        var blurAnimation = new DoubleAnimation
        {
            From = 6,
            To = 18,
            Duration = TimeSpan.FromMilliseconds(650),
            AutoReverse = true,
            RepeatBehavior = RepeatBehavior.Forever
        };

        var opacityAnimation = new DoubleAnimation
        {
            From = 0.25,
            To = 0.9,
            Duration = TimeSpan.FromMilliseconds(650),
            AutoReverse = true,
            RepeatBehavior = RepeatBehavior.Forever
        };

        _glowStoryboard = new Storyboard();

        _glowStoryboard.Children.Add(blurAnimation);
        _glowStoryboard.Children.Add(opacityAnimation);

        Storyboard.SetTarget(blurAnimation, glow);
        Storyboard.SetTargetProperty(
            blurAnimation,
            new PropertyPath(DropShadowEffect.BlurRadiusProperty));

        Storyboard.SetTarget(opacityAnimation, glow);
        Storyboard.SetTargetProperty(
            opacityAnimation,
            new PropertyPath(DropShadowEffect.OpacityProperty));

        _glowStoryboard.Begin();
    }

    private void StopGlow()
    {
        _glowStoryboard?.Stop();
        _glowStoryboard = null;

        if (_visualPath.Effect is DropShadowEffect glow)
        {
            glow.BlurRadius = 6;
            glow.Opacity = 0.2;
        }
    }
}
