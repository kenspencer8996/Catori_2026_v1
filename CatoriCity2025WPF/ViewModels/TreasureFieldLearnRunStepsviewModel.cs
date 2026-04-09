using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.ObjectModel;
namespace CatoriCity2025WPF.ViewModels
{
    public class TreasureFieldLearnRunStepsviewModel : ObservableObject
    {
        public ObservableCollection<LearnedStepModel> LearnedSteps { get; } = new();
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
      }
}
