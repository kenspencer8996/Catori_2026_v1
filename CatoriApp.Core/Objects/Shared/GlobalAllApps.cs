using CatoriUCLibrary.Views.Person;

namespace CatoriApp.Core.Objects.Shared
{
    public sealed record PersonMetadata(
        string Name,
        double Width,
        double Height,
        PersonActivity StartActivity,
        bool IsDragEnabled,
        bool IsArmEditingEnabled,
        string? AvatarSettingsJson,
        PersonRole Role = PersonRole.Avatar);

    public class GlobalAllApps
    {
        public static string _imageFolder = "C:\\Development\\Gaming\\Catori2026\\Catori_2026_v1\\Images";
        public static bool showDebugInfo = true;
        public static bool LearnMode { get; set; } = false;
         public static bool IsDeveloperUser()
        {
            return Environment.UserName.Equals(
                "kensp",
                StringComparison.OrdinalIgnoreCase);
        }
        public static void WriteDebugInfo(string message)
        {
            if (showDebugInfo)
            {
                System.Diagnostics.Debug.WriteLine(message);
            }
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

        public static PersonUC GetPerson(PersonMetadata metadata)
        {
            ArgumentNullException.ThrowIfNull(metadata);
            var person = new PersonUC
            {
                Name = ToElementName(metadata.Name),
                Role = metadata.Role,
                Width = metadata.Width,
                Height = metadata.Height,
                CurrentActivity = metadata.StartActivity,
                IsDragEnabled = metadata.IsDragEnabled,
                IsArmEditingEnabled = metadata.IsArmEditingEnabled
            };
            if (!string.IsNullOrWhiteSpace(metadata.AvatarSettingsJson))
            {
                person.ApplySettings(PersonAvatarSettings.FromJson(metadata.AvatarSettingsJson));
            }
            return person;
        }

        private static string ToElementName(string? value)
        {
            string name = new((value ?? string.Empty)
                .Select(character => char.IsLetterOrDigit(character) || character == '_'
                    ? character
                    : '_')
                .ToArray());
            if (string.IsNullOrWhiteSpace(name))
                return "Person";
            return char.IsDigit(name[0]) ? $"Person_{name}" : name;
        }

       
    }
}


