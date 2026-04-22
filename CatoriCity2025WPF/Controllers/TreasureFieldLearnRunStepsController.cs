using CatoriCity2025WPF.Objects.Arguments;
using CatoriCity2025WPF.Views.Controls.Treasure;

namespace CatoriCity2025WPF.Controllers
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
