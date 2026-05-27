namespace CatoriApp.Core.Objects.Messages
{
    public class HouseSoldMessage
    {
        public HouseViewModel House;
        public HouseSoldMessage(HouseViewModel house)
        {
            House = house;
        }
    }
}

