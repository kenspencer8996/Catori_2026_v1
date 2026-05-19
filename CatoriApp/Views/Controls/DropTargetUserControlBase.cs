using CatoriApp.Objects.DragDrop;
using System.Windows.Media.Animation;

public abstract class DropTargetUserControlBase : UserControl, IDropTarget
{
    protected Border HighlightBorder;

    public DropTargetUserControlBase()
    {
        // Ensure the control participates in hit-testing and layout
        Background = Brushes.Transparent;

        // Optional: a border for highlight glow
        HighlightBorder = new Border
        {
            Background = Brushes.Transparent,
            BorderBrush = Brushes.Transparent,
            BorderThickness = new Thickness(3),
            CornerRadius = new CornerRadius(4),
            IsHitTestVisible = false
        };

        Loaded += (s, e) =>
        {
            if (Content is UIElement child)
            {
                // Wrap existing content in a Grid so we can overlay highlight
                var grid = new Grid();
                Content = null;
                grid.Children.Add(child);
                grid.Children.Add(HighlightBorder);
                Content = grid;
            }
        };
    }

    // ---------------------------------------------------------
    // REQUIRED BY IDropTarget
    // ---------------------------------------------------------

    public abstract bool CanDrop(IDraggable draggable);

    public abstract void OnDrop(IDraggable draggable);

    // Option 3: each target defines its own snap point
    public virtual Point GetSnapPoint(IDraggable draggable)
    {
        // Default: snap to top-left of this control
        //double x = Canvas.GetLeft(this);
        //double y = Canvas.GetTop(this);
        FrameworkElement root = this;

        // Walk up until we hit the Canvas child
        while (root.Parent is FrameworkElement feParent && !(feParent is Canvas))
            root = feParent;

        var feDragged = (FrameworkElement)draggable.Visual;

        double x = Canvas.GetLeft(root) + (root.ActualWidth - feDragged.ActualWidth) / 2;
        double y = Canvas.GetTop(root) + (root.ActualHeight - feDragged.ActualHeight) / 2;

        return new Point(x, y);
    }

    // ---------------------------------------------------------
    // HIGHLIGHTING
    // ---------------------------------------------------------

    public virtual void HighlightOn()
    {
        HighlightBorder.BorderBrush = Brushes.Gold;

        var anim = new DoubleAnimation
        {
            From = 0,
            To = 1,
            Duration = TimeSpan.FromMilliseconds(150),
            EasingFunction = new QuadraticEase { EasingMode = EasingMode.EaseOut }
        };

        HighlightBorder.BeginAnimation(Border.OpacityProperty, anim);
    }

    public virtual void HighlightOff()
    {
        var anim = new DoubleAnimation
        {
            From = HighlightBorder.Opacity,
            To = 0,
            Duration = TimeSpan.FromMilliseconds(150),
            EasingFunction = new QuadraticEase { EasingMode = EasingMode.EaseOut }
        };

        HighlightBorder.BeginAnimation(Border.OpacityProperty, anim);
    }

    // ---------------------------------------------------------
    // REJECT DROP (optional animation)
    // ---------------------------------------------------------

    public virtual void RejectDrop(IDraggable draggable)
    {
        // Simple shake animation
        var element = draggable.Visual;

        var anim = new DoubleAnimationUsingKeyFrames();
        anim.KeyFrames.Add(new DiscreteDoubleKeyFrame(0, KeyTime.FromTimeSpan(TimeSpan.FromMilliseconds(0))));
        anim.KeyFrames.Add(new DiscreteDoubleKeyFrame(-5, KeyTime.FromTimeSpan(TimeSpan.FromMilliseconds(50))));
        anim.KeyFrames.Add(new DiscreteDoubleKeyFrame(5, KeyTime.FromTimeSpan(TimeSpan.FromMilliseconds(100))));
        anim.KeyFrames.Add(new DiscreteDoubleKeyFrame(0, KeyTime.FromTimeSpan(TimeSpan.FromMilliseconds(150))));

        element.RenderTransform = new TranslateTransform();
        element.RenderTransform.BeginAnimation(TranslateTransform.XProperty, anim);
    }
}

