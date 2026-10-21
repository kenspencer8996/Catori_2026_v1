using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace CatoriUCLibrary.Views;

public partial class SimpleCar : UserControl
{
    private static readonly BitmapSource[] Frames = LoadFrames();
    private Point? _previousPosition;
    private Canvas? _previousCanvas;
    public event EventHandler? StartMovementRequested;
    public static readonly DependencyProperty HeadingProperty = DependencyProperty.Register(
        nameof(Heading), typeof(CarHeading), typeof(SimpleCar),
        new PropertyMetadata(CarHeading.Right, HeadingChanged));
    public static readonly DependencyProperty FollowPathDirectionProperty = DependencyProperty.Register(
        nameof(FollowPathDirection), typeof(bool), typeof(SimpleCar), new PropertyMetadata(true));

    public SimpleCar()
    {
        InitializeComponent();
        ShowHeading();
        Loaded += (_, _) =>
        {
            _previousPosition = null;
            CompositionTarget.Rendering += UpdateDirection;
        };
        Unloaded += (_, _) =>
        {
            CompositionTarget.Rendering -= UpdateDirection;
            _previousPosition = null;
        };
    }

    public CarHeading Heading
    {
        get => (CarHeading)GetValue(HeadingProperty);
        set => SetValue(HeadingProperty, value);
    }

    public bool FollowPathDirection
    {
        get => (bool)GetValue(FollowPathDirectionProperty);
        set => SetValue(FollowPathDirectionProperty, value);
    }

    private static void HeadingChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
    {
        ((SimpleCar)sender).ShowHeading();
    }

    private void ShowHeading()
    {
        if (DirectionalCarImage != null)
        {
            DirectionalCarImage.Source = Frames[(int)Heading];
        }
    }

    private void UpdateDirection(object? sender, EventArgs args)
    {
        if (!FollowPathDirection || !IsVisible)
        {
            _previousPosition = null;
            return;
        }
        DependencyObject? parent = VisualTreeHelper.GetParent(this);
        while (parent != null && parent is not Canvas)
        {
            parent = VisualTreeHelper.GetParent(parent);
        }
        if (parent is not Canvas canvas)
        {
            return;
        }
        Point position = TranslatePoint(new Point(ActualWidth / 2, ActualHeight / 2), canvas);
        if (!ReferenceEquals(canvas, _previousCanvas))
        {
            _previousPosition = null;
            _previousCanvas = canvas;
        }
        if (_previousPosition is Point previous)
        {
            Vector movement = position - previous;
            if (movement.LengthSquared > 0.04)
            {
                // Keep the previous view near diagonals to avoid rapid frame changes.
                if (Math.Abs(movement.X) > Math.Abs(movement.Y) * 1.15)
                {
                    SetCurrentValue(HeadingProperty, movement.X > 0 ? CarHeading.Right : CarHeading.Left);
                }
                else if (Math.Abs(movement.Y) > Math.Abs(movement.X) * 1.15)
                {
                    SetCurrentValue(HeadingProperty, movement.Y > 0 ? CarHeading.Down : CarHeading.Up);
                }
            }
        }
        _previousPosition = position;
    }

    private void StartMovement_Click(object sender, RoutedEventArgs args)
    {
        StartMovementRequested?.Invoke(this, EventArgs.Empty);
    }

    private static BitmapSource[] LoadFrames()
    {
        var atlas = new BitmapImage(new Uri("pack://application:,,,/CatoriUCLibrary;component/Images/SimpleCarDirections.png"));
        int width = atlas.PixelWidth / 2;
        int height = atlas.PixelHeight / 2;
        var right = new CroppedBitmap(atlas, new Int32Rect(0, 0, width, height));
        var left = new CroppedBitmap(atlas, new Int32Rect(width, 0, width, height));
        var down = new CroppedBitmap(atlas, new Int32Rect(0, height, width, height));
        var up = new CroppedBitmap(atlas, new Int32Rect(width, height, width, height));
        return new[] { Trim(right), Trim(down), Trim(left), Trim(up) };
    }

    private static BitmapSource Trim(BitmapSource source)
    {
        var converted = new FormatConvertedBitmap(source, PixelFormats.Bgra32, null, 0);
        int stride = converted.PixelWidth * 4;
        byte[] pixels = new byte[stride * converted.PixelHeight];
        converted.CopyPixels(pixels, stride, 0);
        int left = converted.PixelWidth;
        int top = converted.PixelHeight;
        int right = 0;
        int bottom = 0;
        for (int y = 0; y < converted.PixelHeight; y++)
        {
            for (int x = 0; x < converted.PixelWidth; x++)
            {
                if (pixels[y * stride + x * 4 + 3] < 32)
                {
                    continue;
                }
                left = Math.Min(left, x);
                top = Math.Min(top, y);
                right = Math.Max(right, x);
                bottom = Math.Max(bottom, y);
            }
        }
        var frame = new CroppedBitmap(converted, new Int32Rect(left, top, right - left + 1, bottom - top + 1));
        frame.Freeze();
        return frame;
    }
}
