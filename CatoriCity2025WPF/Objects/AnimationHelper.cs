using System.Windows;
using System.Windows.Media.Animation;

namespace CatoriCity2025WPF.Objects
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
    }
}
