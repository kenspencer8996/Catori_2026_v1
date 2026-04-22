using CatoriCity2025WPF.Controllers;
using CatoriCity2025WPF.Objects.Arguments;
using CatoriCity2025WPF.Services;
using CommunityToolkit.Mvvm.Messaging;

namespace CatoriCity2025WPF.Views.Controls.Treasure
{
    /// <summary>
    /// Interaction logic for TreasureFieldLearnRunStepsControl.xaml
    /// </summary>
    public partial class TreasureFieldLearnRunStepsControl : UserControl
    {
        //public TreasureFieldLearnRunStepsviewModel _model;
        LearnedStepService learnedStepService = new LearnedStepService();
        TreasureFieldLearnRunStepsController _controller;
        LearnedStepModel _selectedStep;
        public event EventHandler<LearnedStepRunArgument> RunSteps;
        TreasureStepArgs args
        {
            set
            {
                if (value != null)
                {
                    _controller.AddStep(value);
                }
            }
        }
        public TreasureFieldLearnRunStepsControl(TreasureFieldLearnRunStepsviewModel model)
        {
            InitializeComponent();
            _controller = new TreasureFieldLearnRunStepsController(this, model);
            LoadSteps();
            if (Environment.UserName.ToLower() != "kensp")
                numberOfSpotsToRunTextbox.Visibility = Visibility.Hidden;
            else
                numberOfSpotsToRunTextbox.Text = "1";

            WeakReferenceMessenger.Default.Register<TreasureStepArgs>(this, (r, m) =>
            {
                args = m;
            });
        }
        private  void LoadSteps()
        {//“You taught how to handle a treasure spot!”
            var results =  learnedStepService.GetAll();
            _controller._model.LearnedSteps.Clear();
            foreach (var item in results)
            {
                _controller._model.LearnedSteps.Add(item);
            }
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            learnedStepService.UpdateAsync(_controller._model.LearnedSteps.ToList());
        }

        private void RunAllButton_Click(object sender, RoutedEventArgs e)
        {
            LearnedStepRunArgument args = new LearnedStepRunArgument();
            args.LearnedSteps = _controller._model.LearnedSteps;
            args.NumberOfSpotsToRun = int.TryParse(numberOfSpotsToRunTextbox.Text, out int result) ? result : 0;
            if (RunSteps != null)
                RunSteps?.Invoke(this, args);
        }    

        private void RunStepButton_Click(object sender, RoutedEventArgs e)
        {
            LearnedStepRunArgument args = new LearnedStepRunArgument();
            args.LearnedSteps = _controller._model.LearnedSteps;
            args.NumberOfSpotsToRun = int.TryParse(numberOfSpotsToRunTextbox.Text, out int result) ? result : 0;
            args.RunType = "SingleStep";
            if (RunSteps != null)
                RunSteps?.Invoke(this, args);
        }

        private void RemoveButton_Click(object sender, RoutedEventArgs e)
        {
            if (_selectedStep != null)
            {
                int i = _controller._model.LearnedSteps.IndexOf(_selectedStep);
                _controller._model.LearnedSteps.Remove(_selectedStep);
            }
        }

       
        private void SelectAllCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            foreach (var step in _controller._model.LearnedSteps)
            {
                if (step.IsChecked == false)
                    step.IsChecked = true;
                else
                    step.IsChecked = false;
            }
        }

        private void RecordedStepsListbox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            _selectedStep = (LearnedStepModel)RecordedStepsListbox.SelectedItem;

        }
    }
}
