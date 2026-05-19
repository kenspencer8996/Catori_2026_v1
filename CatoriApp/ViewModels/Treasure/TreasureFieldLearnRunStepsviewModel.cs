using CatoriApp.Objects.Arguments;
using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.ObjectModel;
namespace CatoriApp.ViewModels.Treasure
{
    public class TreasureFieldLearnRunStepsviewModel : ObservableObject
    {
        private ObservableCollection<LearnedStepModel> _learnedSteps = new();
        public ObservableCollection<LearnedStepModel> LearnedSteps
        {
            get => _learnedSteps;
            set => SetProperty(ref _learnedSteps, value);
        }
        public bool IsStickyAcrossRuns { get; set; } = true;

        private LearnedStepModel? _selectedStep;
        public LearnedStepModel? SelectedStep
        {
            get => _selectedStep;
            set => SetProperty(ref _selectedStep, value);
        }

        private bool _isAutoRunEnabled;
        public bool IsAutoRunEnabled
        {
            get => _isAutoRunEnabled;
            set => SetProperty(ref _isAutoRunEnabled, value);
        }

        private decimal _cashtreasure;
        public decimal CashTreasure
        {
            get => _cashtreasure;
            set
            {
                if (SetProperty(ref _cashtreasure, value))
                    OnPropertyChanged(nameof(CashTreasure));
            }
        }
        public string CashDisplay => $"Cash: {CashTreasure:C}";

        internal void AddStep(TreasureStepArgs value)
        {
            if (value.ClearList == true)
            {
                LearnedSteps.Clear();
            }
            LearnedStepModel learnedStep = new LearnedStepModel();
            learnedStep = GetLearnedStepModel(value);
           
            // LearnedSteps.Add(learnedStepwalk);
            LearnedSteps.Add(learnedStep);
            cLogger.Log($"Adding step: {value.StepNumber} - {value.Name} - {LearnedSteps.Count}", value.StepNumber.ToString(), value.Name, LearnedSteps.Count);

        }
        private LearnedStepModel GetLearnedStepModel(TreasureStepArgs value)
        {
            LearnedStepModel learnedStep = new LearnedStepModel();
            learnedStep.Name = value.Name;
            learnedStep.TreasureStep = value.TreasureStep;
            switch (value.TreasureStep)
            {
                case TreasureStepEnum.WalkToTreasureSpot:
                    learnedStep.DisplayName = "Walk to Treasure Spot";
                    learnedStep.StepNumber = 1;
                    break;
                  case TreasureStepEnum.WalkToWorkbench:
                    learnedStep.DisplayName = "Workbench";
                    learnedStep.StepNumber = 3;
                    break;
                  case TreasureStepEnum.Bank:
                    learnedStep.DisplayName = "Bank";
                    learnedStep.StepNumber = 5;
                    break;
                default:
                    break;
            }
            return learnedStep;
        }
    }
}


