using System;
using System.Collections.Generic;
using System.Text;

namespace CatoriApp.Core.Objects
{
    public class MediaCommon
    {
        public static System.Windows.Media.Brush GetBrushForMajorItemType(string majorItemType)
        {
            switch (majorItemType)
            {
                case "Conveyor Run":
                    return System.Windows.Media.Brushes.Blue;
                case "Robot Zone":
                    return System.Windows.Media.Brushes.AliceBlue;
                case "Table Zone":
                    return System.Windows.Media.Brushes.Red;
                case "Drop zone":
                    return System.Windows.Media.Brushes.DarkRed;
                default:
                    return System.Windows.Media.Brushes.LightCoral;
            }
        }
    }
}
