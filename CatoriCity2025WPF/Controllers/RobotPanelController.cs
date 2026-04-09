using CatoriCity2025WPF.Views.Controls.Factory.FactoryInteriors;
using System;
using System.Collections.Generic;
using System.Text;

namespace CatoriCity2025WPF.Controllers
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
