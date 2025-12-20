using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CatoriCity2025WPF.Objects
{
    internal class MathHelper
    {
        public static double CheckvalidDouble(double value)
        {
            double x = 0;
            if (Double.IsNaN(value))
            {
                value = 0; ;
            }
            x = value;
            return x;
        }
    }
}
