using System.Globalization;

namespace CatoriApp.Core.Objects.Shared
{
    public class CatoriStringConverter
    { /// <summary>
      /// Safely converts a string to a double.
      /// </summary>
      /// <param name="input">The string to convert.</param>
      /// <param name="result">The converted double value (0 if conversion fails).</param>
      /// <returns>True if conversion succeeded, false otherwise.</returns>
        public static double ConvertToDouble(string input)
        {
            double outresult;
            double.TryParse(
                input,
                NumberStyles.Float | NumberStyles.AllowThousands,
                CultureInfo.InvariantCulture,
                out outresult
            );
            // Use InvariantCulture to avoid locale-specific decimal issues
            return outresult;
        }
    }
}
