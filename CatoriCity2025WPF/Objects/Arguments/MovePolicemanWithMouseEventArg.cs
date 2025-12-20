using CatoriCity2025WPF.Views.Controls;

namespace CatoriCity2025WPF.Objects.Arguments
{
    public class MovePolicemanWithMouseEventArg
    {
        public bool MoveNext = false;
        public double _x;
        public double _y;
        internal PolicemanControl _PolicemanControl;
        internal MovePolicemanWithMouseEventArg(double x, double y,
            PolicemanControl policeman)
        {
            _x = x;
            _y = y;
            _PolicemanControl = policeman;
        }
       
    }
}
