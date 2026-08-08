using System.Runtime.ConstrainedExecution;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace CatoriUCLibrary.Views.RobotArm
{
    /// <summary>
    /// Interaction logic for RobotArmSegmentontrol.xaml
    /// </summary>
    public partial class RobotArmSegmentontrol : UserControl
    {
        public event EventHandler<SegmentMouseDownArgs> SegmentMouseDown;
  
        public double SegmentLength { get; set; } = 124;

        public RobotArmSegmentontrol()
        {
            InitializeComponent();
        }
        public void SetSegmentImage(string imagePath)
        {
            string uri = Imagehelper.GetImagePath(imagePath);
            SegmentImage.Source = new BitmapImage(new Uri(uri, UriKind.Absolute));
        }

        public double GetImageLength()
        {
            return SegmentImage.Source.Width;
        }   
        public double SegmentAngle
        {
            get
            {
                //return SegmentRotate.Angle;
                return SegmentRotate.Angle;
            }
            set
            {
                SegmentRotate.Angle = value;
                //SegmentRotate.Angle = value;
            }
        }
        private void UserControl_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            SegmentMouseDownArgs seg = new SegmentMouseDownArgs(this.Name,e);
            SegmentMouseDown?.Invoke(this, seg);
            e.Handled = true;
        }

        private void UserControl_MouseUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            System.Diagnostics.Debug.WriteLine($"Mouse up on {this.Name} angle " + SegmentRotate.Angle);
        }
    } 
}
