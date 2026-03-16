namespace CatoriCity2025WPF.Objects
{
    public class TreasureEntity
    {
        public string Name { get; set; }
        public string ImagePath { get; set; }
        public double Value 
            { 
            get
                { return GetValue(); }
            }
        public double GetValue()
        {
            double value;
            // Implement logic to calculate the value of the treasure based on its properties
            // For example, you could assign a value based on the type of treasure or its rarity
            Random rnd = new Random();
            if (Name.Contains("Cash"))
            {

                value = GlobalAllApps.GetRandomDouble(100,5200);
            }
            else if (Name.Contains("BagOfrocks"))
            {
                value = GlobalAllApps.GetRandomDouble(1, 4);
            }
            else if (Name.Contains("BagOfSticks"))
            {
                value = 1;
            }
            else if (Name.Contains("BagOfValubleTrinkets"))
            {
                value = GlobalAllApps.GetRandomDouble(100, 25000);

            }
            else
            {
                //BagOfValuabeStones
                value = GlobalAllApps.GetRandomDouble(25, 100000);
                ; // Default value for other treasures
            }
            return value;
        }
    }
}
