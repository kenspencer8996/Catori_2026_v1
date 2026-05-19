using CatoriApp.Objects;
using System.Windows.Controls;
using System.Windows.Threading;
namespace CatoriApp.Views.Controls.Locations.Factory.Interiors
{
    /// <summary>
    /// Interaction logic for Factoru1InteriorUC.xaml
    /// </summary>
    public partial class LocationInterior_4UC : UserControl
    {
        readonly DispatcherTimer _animation_timer;
        List <string> robotImagePaths = new List<string>();
        public string SearchPattern { get; set; } = "05*.png";
        public string ImagesFolderLeft = System.IO.Path.Combine(GlobalAllApps.ImageFolder, "Locations", "RobotArms", "Left");
        public string restImageLeft = System.IO.Path.Combine(GlobalAllApps.ImageFolder, "Locations", "RobotArms", "00RobotArmStart.png");


        public string SearchPatternRight { get; set; } = "*.png";
        public string ImagesFolderRight = System.IO.Path.Combine(GlobalAllApps.ImageFolder, "Locations", "RobotArms", "RightDrilling");
        public string restImageRight = System.IO.Path.Combine(GlobalAllApps.ImageFolder, "Locations", "RobotArms", "RightDrilling", "RobotArm2rightDrilling00r.png");
        //C:\Develpoment\Games\Images\Locations\RobotArms\RightDrilling

        public LocationInterior_4UC()
        {
            InitializeComponent();
            _animation_timer = new DispatcherTimer(DispatcherPriority.Normal);
            //RobotLeftUC.Startup(SearchPattern, ImagesFolderLeft,restImageLeft);
            //RobotRightUC.Startup(SearchPatternRight, ImagesFolderRight, restImageRight);
            ////LoadRobotImages();
        }

        public void StartWorking(string workerImagePath)
        {
            //RobotLeftUC.StartWorking();
            //RobotRightUC.StartWorking();
            WorkerImage.Source = UIUtility.GetImageControl(workerImagePath, 10, 5, 0).Source; ;
        }
        public void StopWorking()
        {
            //_animation_timer.Stop();
            //    RobotLeftUC.StopWorking();
            //    RobotRightUC.StopWorking();
        }
    }
}


