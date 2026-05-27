namespace CatoriApp.Core.Objects.Shared
{
    public class GlobalAllApps
    {
        public static string _imageFolder = "C:\\Development\\Gaming\\Catori2026\\Catori_2026_v1\\Images";
        public static bool showDebugInfo = false;
        public static bool LearnMode { get; set; } = false;
        public static PersonViewModel CurrentPerson { get; set; }
        public static List<PersonViewModel> AllPersons { get; set; } = new List<PersonViewModel>();
        public static bool IsDeveloperUser()
        {
            return Environment.UserName.Equals(
                "kensp",
                StringComparison.OrdinalIgnoreCase);
        }
        public static string ImageFolder
        {
            set
            {
                _imageFolder = value;
            }
            get
            {
                return _imageFolder;
            }
        }

        /// <summary>
        /// Generates a random double between minValue (inclusive) and maxValue (exclusive).
        /// </summary>
        /// <param name="minValue">Minimum value (inclusive)</param>
        /// <param name="maxValue">Maximum value (exclusive)</param>
        /// <returns>A random double in the specified range</returns>
        /// <exception cref="ArgumentException">Thrown if minValue >= maxValue</exception>
        public static double GetRandomDouble(double minValue, double maxValue)
        {
            if (minValue >= maxValue)
                throw new ArgumentException("minValue must be less than maxValue.");

            Random random = new Random(); // For better randomness, reuse this in real apps
            return random.NextDouble() * (maxValue - minValue) + minValue;
        }

       
    }
}


