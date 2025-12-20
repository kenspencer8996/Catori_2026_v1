using CatoriCity2025WPF.ViewModels;
using CatoriServices.Objects;
using System.Windows.Media.Animation;

namespace CatoriCity2025WPF.Objects
{
    public class PathPositionModel
    {
        public string LandscapeName { get; set; } = string.Empty;   
        public double startx;
        public double starty;
        public double endx;
        public double endy;
        public double width;
        public double centerx;
        public double centery;
        public double durationseconds = GlobalStuff.mainWindowViewModel.BadGuyTravelSpeed;//ms
          
        public DoubleAnimation LeftAnimation ;
        public DoubleAnimation TopAnimation;
        public void SetAnimations(double instartx, double instarty)
        {
            if (durationseconds < 1000)
            {
                durationseconds = durationseconds * 1000;
            }
            LeftAnimation = AnimationHelper.GetDoubleAnimation(
                            instartx, centerx, durationseconds);
            TopAnimation = AnimationHelper.GetDoubleAnimation(
                instarty, centery, durationseconds);
        }

        public void SetCenter()
        {
            centerx = (endx) + (width / 2);
            centery = (endy) +  50;
            cLogger.Log($"SetCenter {LandscapeName} endx {endx} endy {endy} centerx {centerx} centery {centery}");
        }
        public string GeometryType { get; set; } = "Line";  
    }
}
