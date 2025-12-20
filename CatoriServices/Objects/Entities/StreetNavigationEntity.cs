using CatoriCity2025WPF.Objects;

namespace CityAppServices.Objects.Entities
{
    public class StreetNavigationEntity
    {
        public StreetsEnum StreetName {  get; set; }
        public LocationXYEntity StreetLocation { get; set; }
        public LotEntity LotToTravelTo { get; set; }
        public double StartX { get; set; }
        public double StartY { get; set; }
        public int Index { get; set; }
        public PositionsEWNSEnum TravelDirection { get; set; }
        public StreetTraverseEnum StreetTraverse { get; set; }
        public void AddTravelInfo(StreetsEnum streetName, double x,
            double y, StreetTraverseEnum streetTraverse,
            PositionsEWNSEnum travelDirection, double startX, 
            double startY, int index)
        {
            StreetName = streetName;
            StreetLocation = new LocationXYEntity();
            StreetTraverse= streetTraverse;
            StreetLocation.x = x;
            StreetLocation.y = y;
            TravelDirection = travelDirection;
            StartX = startX;
            StartY = startY;
            Index = index;

        }

    }
}
