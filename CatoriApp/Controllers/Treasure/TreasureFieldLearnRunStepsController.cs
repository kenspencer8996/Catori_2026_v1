using CatoriApp.Objects.Arguments;
using CatoriApp.Views.Controls.Treasure;
namespace CatoriApp.Controllers.Treasure
{
    public class TreasureFieldLearnRunStepsController
    {
        TreasureFieldLearnRunStepsControl _view;
        
        public TreasureFieldLearnRunStepsviewModel _model;
        public TreasureFieldLearnRunStepsController(
            TreasureFieldLearnRunStepsControl view, TreasureFieldLearnRunStepsviewModel model)
        {
            _view = view;
            _model = model;
            _view.DataContext = _model;
        }

        internal void AddStep(TreasureStepArgs value)
        {
            _model.AddStep(value);
        }
    }
}


