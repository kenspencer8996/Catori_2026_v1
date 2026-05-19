using CatoriApp.Views.Controls.Robots;
namespace CatoriApp.Controllers.Robots
{
    public class RobotPanelController
    {
        RobotControlPanel _view;
        public RobotPanelController(RobotControlPanel view)
        {
            _view = view;
        }
        //public void LoadData(
        //    string robotName,
        //    List<string> inputAItems,
        //    List<string> inputBItems,
        //    List<string> outputItems,
        //    List<string> modeItems)
        //{
        //    _view.LoadData(robotName, inputAItems, inputBItems, outputItems, modeItems);
        //}
    }
}


