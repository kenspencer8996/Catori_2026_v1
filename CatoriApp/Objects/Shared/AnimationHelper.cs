using System.Windows;
using System.Windows.Media.Animation;
namespace CatoriApp.Objects.Shared
{
    internal class AnimationHelper
    {
        public static DoubleAnimation GetDoubleAnimation(double from, double to, 
            double durationMilliseconds)
        {
            if ( double.IsNaN(to))
                to = 0;

            if (double.IsNaN(from))
                from = 0;

            if (double.IsNaN(durationMilliseconds))
                durationMilliseconds = 0;
            DoubleAnimation doubleAnimation = new DoubleAnimation();
            try
            {
                doubleAnimation = new DoubleAnimation
                {
                    From = from,
                    To = to,
                    Duration = new Duration(TimeSpan.FromMilliseconds(durationMilliseconds)),
                    FillBehavior = FillBehavior.HoldEnd
                };
            }
            catch (Exception  ex)
            {

                throw;
            }
            return doubleAnimation;
        }
        public static DoubleAnimation GetDoubleAnimationWIthEasing(double from, double to,
          double durationMilliseconds)
        {
            if (double.IsNaN(to))
                to = 0;

            if (double.IsNaN(from))
                from = 0;

            if (double.IsNaN(durationMilliseconds))
                durationMilliseconds = 0;
            DoubleAnimation doubleAnimation = new DoubleAnimation();
            try
            {
                doubleAnimation = new DoubleAnimation
                {
                    From = from,
                    To = to,
                    Duration = new Duration(TimeSpan.FromMilliseconds(durationMilliseconds)),
                    FillBehavior = FillBehavior.HoldEnd
                };
                doubleAnimation.EasingFunction = new ExponentialEase
                {
                    EasingMode = EasingMode.EaseOut,
                    Exponent = 5
                };
            }
            catch (Exception ex)
            {

                throw;
            }
            return doubleAnimation;
        }
    }


}



