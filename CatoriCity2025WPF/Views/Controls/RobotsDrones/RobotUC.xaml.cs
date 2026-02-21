using CatoriCity2025WPF.Controllers;
using CatoriCity2025WPF.Objects;
using System.Windows.Controls;

namespace CatoriCity2025WPF.Views.Controls
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
