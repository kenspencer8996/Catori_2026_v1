using System.Windows.Input;

namespace CatoriCity2025WPF.Views.Controls.Robots
{
    public class SegmentMouseDownArgs
    {
        public SegmentMouseDownArgs(string segmentName, MouseButtonEventArgs args)
        {
            SegmentName = segmentName;
            MouseArgs = args;
        }
        public string SegmentName { get; set; }
        public MouseButtonEventArgs MouseArgs { get; set; }

    }
}
