using CatoriCity2025WPF.Objects;
using CatoriCity2025WPF.Views.Controls;

namespace CatoriCity2025WPF.Controllers
{
    public class RobotUCController: ControllerBase
    {
  
        RobotUC _view;
        int _imageindex = 0;
        public string SearchPattern = "" ;
        public string ImagesFolder = "";
        public string RestImmage = "";
        public RobotUCController(RobotUC view)
        {
            _view = view ?? throw new ArgumentNullException(nameof(view));
            // LoadImages();


            _view.RobotImage.Source = UIUtility.GetImageControl(RestImmage, 10, 5, 0).Source;
        }
        public void Startup()
        {
            base.SearchPattern = SearchPattern;
            base.ImagesFolder = ImagesFolder;
            base.Startup(_view.RobotImage);
        }   
        public void StartWorking()
        {
            base.StartAnimation();
        }
        public void StopWorking()
        {
            base.StopAnimation();
            _view.RobotImage.Source = UIUtility.GetImageControl(RestImmage, 10, 5, 0).Source;
        }
     
    }
}
