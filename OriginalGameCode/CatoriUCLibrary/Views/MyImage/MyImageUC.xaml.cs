using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media.Imaging;

namespace CatoriUCLibrary.Views.MyImage;

public partial class MyImageUC : UserControl
{
    public static readonly DependencyProperty ImagePathProperty = DependencyProperty.Register(
        nameof(ImagePath), typeof(string), typeof(MyImageUC),
        new PropertyMetadata(null, (_, args) => ((MyImageUC)_).LoadImage(args.NewValue as string)));

    public MyImageUC()
    {
        InitializeComponent();
        Width = 220;
        Height = 160;
    }

    public string? ImagePath
    {
        get => (string?)GetValue(ImagePathProperty);
        set => SetValue(ImagePathProperty, value);
    }

    public void LoadImage(string? path)
    {
        if (string.IsNullOrWhiteSpace(path)) { ImageView.Source = null; return; }
        BitmapImage image = new();
        image.BeginInit();
        image.CacheOption = BitmapCacheOption.OnLoad;
        image.UriSource = Path.IsPathRooted(path) ? new Uri(path, UriKind.Absolute) : new Uri(path, UriKind.RelativeOrAbsolute);
        image.EndInit();
        image.Freeze();
        ImageView.Source = image;
    }

    private void MoveThumb_DragDelta(object sender, DragDeltaEventArgs e)
    {
        if (Parent is Canvas)
        {
            Canvas.SetLeft(this, Normalize(Canvas.GetLeft(this)) + e.HorizontalChange);
            Canvas.SetTop(this, Normalize(Canvas.GetTop(this)) + e.VerticalChange);
        }
        else
        {
            Margin = new Thickness(Margin.Left + e.HorizontalChange, Margin.Top + e.VerticalChange,
                Margin.Right - e.HorizontalChange, Margin.Bottom - e.VerticalChange);
        }
    }

    private void ResizeThumb_DragDelta(object sender, DragDeltaEventArgs e)
    {
        Width = Math.Max(MinWidth, ActualWidth + e.HorizontalChange);
        Height = Math.Max(MinHeight, ActualHeight + e.VerticalChange);
    }

    private void RemoveButton_Click(object sender, RoutedEventArgs e)
    {
        if (Parent is Panel panel) panel.Children.Remove(this);
    }

    private static double Normalize(double value)
    {
        return double.IsNaN(value) ? 0 : value;
    }
}
