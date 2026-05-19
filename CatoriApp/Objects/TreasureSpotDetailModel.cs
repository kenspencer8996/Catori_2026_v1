using CatoriApp.Views.Controls.Treasure;
namespace CatoriApp.Objects
{
    public class TreasureSpotDetailModel
    {
        public TreasureSpotDetailModel()
        {
            Random random = new Random();
            double minValue = 10;
            double maxValue = 10000;
            TreasureValue = (decimal)(minValue + (random.NextDouble() * (maxValue - minValue)));
        }
        public TreasureSpotControl Treasurespot;
        public decimal TreasureValue { get; set; }
        public Point GetSpotLocation()
        {
            Point point = new Point(0,0);
            if (Treasurespot != null)
            {
                double left = Canvas.GetLeft(Treasurespot);
                double top = Canvas.GetTop(Treasurespot);
                left = left = Treasurespot.ActualWidth / 2 ;
                top = top + Treasurespot.ActualHeight / 2;
                point = new Point(left,top);
            }
            return point;
        }
    }
}


