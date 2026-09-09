using System.Windows.Media.Animation;
namespace CatoriApp.Game.Controllers.City
{
    public class AnimationController
    {
        public Storyboard MoveXYOnControlToAsync(
              FrameworkElement control,
              double xpos,double ypos,TimeSpan duration)
        {
            var tcs = new TaskCompletionSource<bool>();

            double baseLeft = Canvas.GetLeft(control);
            double baseTop = Canvas.GetTop(control);
            if (double.IsNaN(baseLeft)) baseLeft = 0;
            if (double.IsNaN(baseTop)) baseTop = 0; 
            //double targetX = destination.X - baseLeft;
            //double targetY = destination.Y - baseTop;
            double targetX = xpos;
            double targetY = ypos;
            var animX = new DoubleAnimation
            {
                To = targetX,
                Duration = new Duration(duration),
                FillBehavior = FillBehavior.HoldEnd
            };

            var animY = new DoubleAnimation
            {
                To = targetY,
                Duration = new Duration(duration),
                FillBehavior = FillBehavior.HoldEnd
            };

            var sb = new Storyboard();
            Storyboard.SetTargetProperty(animX, new PropertyPath("(Canvas.Left)"));
            Storyboard.SetTarget(animX, control);

            Storyboard.SetTargetProperty(animY, new PropertyPath("(Canvas.Top)"));
            Storyboard.SetTarget(animY, control);

            sb.Children.Add(animX);
            sb.Children.Add(animY);

            sb.Completed += (s, e) => tcs.TrySetResult(true);
            sb.Begin();

            return sb;
        }

        public Task MoveControlAlongPathAsync(
            TranslateTransform translate,PathGeometry path,TimeSpan duration)
        {
            var tcs = new TaskCompletionSource<bool>();

            var xAnim = new DoubleAnimationUsingPath
            {
                PathGeometry = path,
                Source = PathAnimationSource.X,
                Duration = new Duration(duration),
                FillBehavior = FillBehavior.HoldEnd
            };

            var yAnim = new DoubleAnimationUsingPath
            {
                PathGeometry = path,
                Source = PathAnimationSource.Y,
                Duration = new Duration(duration),
                FillBehavior = FillBehavior.HoldEnd
            };

            var sb = new Storyboard();

            Storyboard.SetTarget(xAnim, translate);
            Storyboard.SetTargetProperty(xAnim, new PropertyPath(TranslateTransform.XProperty));

            Storyboard.SetTarget(yAnim, translate);
            Storyboard.SetTargetProperty(yAnim, new PropertyPath(TranslateTransform.YProperty));

            sb.Children.Add(xAnim);
            sb.Children.Add(yAnim);

            sb.Completed += (s, e) => tcs.TrySetResult(true);
            sb.Begin();

            return tcs.Task;
        }
    }
}


