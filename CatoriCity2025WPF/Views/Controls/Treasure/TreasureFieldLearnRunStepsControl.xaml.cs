namespace CatoriCity2025WPF.Views.Controls.Treasure
{
    /// <summary>
    /// Interaction logic for TreasureFieldLearnRunStepsControl.xaml
    /// </summary>
    public partial class TreasureFieldLearnRunStepsControl : UserControl
    {
        TreasureFieldLearnRunStepsviewModel _model =  new TreasureFieldLearnRunStepsviewModel();
        LearnedStepService learnedStepService = new LearnedStepService();
        public TreasureFieldLearnRunStepsControl(TreasureFieldLearnRunStepsviewModel model)
        {
            InitializeComponent();

            LoadSteps();
            DataContext = _model;
        }
        private void LoadSteps()
        {//“You taught how to handle a treasure spot!”
            _model =learnedStepService.LoadStepsIntoViewModel(_model, "FactoryInteriorName");
        }
    }
}
