using CatoriApp.Game.Controllers;
using CatoriApp.Core.Objects;
using System.Windows.Controls;
namespace CatoriApp.Game.Views.Controls.RobotsDrones
{
    /// <summary>
    /// Interaction logic for Crane1UC.xaml
    /// </summary>
    public partial class RobotUC : UserControl
    {
        RobotUCController controller;
        public RobotUC()
        {
            InitializeComponent();
            controller = new RobotUCController(this);
           
        }
        public void Startup(string SearchPattern, string ImagesFolder, string RestImmage)
        {
            controller.SearchPattern = SearchPattern;
            controller.ImagesFolder = ImagesFolder;
            controller.RestImmage = RestImmage;
            controller.Startup();
        }
     
       

        public void StartWorking()
        {
            controller.StartWorking();
        }
        public void StopWorking()
        {
            controller.StopWorking();
        }
    }
}


