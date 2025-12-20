using CatoriCity2025WPF.Views.Controls;

namespace CatoriCity2025WPF.Objects.Arguments
{

    public class BadPersonMoveFiredEventArg
    {
        public bool MoveNext = false;
        public double _x;
        public double _y;
        public PersonViewModel Badguy;
        internal BadPersonControl BadGuyContent;
        internal BadPersonMoveFiredEventArg(double x, double y,
            PersonViewModel badguy, BadPersonControl badGuyContent)
        {
            _x = x;
            _y = y;
            Badguy = badguy;
            BadGuyContent = badGuyContent;
        }
        //public Rect GetRectCoordinates()
        //{
        //    Rect locRec = new Rect();
        //    locRec.X = _x;
        //    locRec.Y = _y;
        //    locRec.Height = AbsoluteLayout.AutoSize;
        //    locRec.Width = AbsoluteLayout.AutoSize;
        //    return locRec;
        //}
    }
}
