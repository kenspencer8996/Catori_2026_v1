using System.ComponentModel;
using System.Runtime.CompilerServices;
namespace CatoriApp.Game.ViewModels.Treasure
{
    public class LearnedStepModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        protected bool SetField<T>(ref T field, T value, [CallerMemberName] string? propertyName = null)
        {
            if (EqualityComparer<T>.Default.Equals(field, value)) return false;
            field = value;
            OnPropertyChanged(propertyName);
            return true;
        }

        private TreasureStepEnum _treasurestep;
        public TreasureStepEnum TreasureStep
        {
            get => _treasurestep;
            set
            {
                SetField(ref _treasurestep, value);
            }
        }

        private int _learnedStepId;
        public int LearnedStepId
        {
            get => _learnedStepId;
            set => SetField(ref _learnedStepId, value);
        }

        private string _name;
        public string Name
        {
            get => _name;
            set => SetField(ref _name, value);
        }

        private int _stepNumber;
        public int StepNumber
        {
            get => _stepNumber;
            set => SetField(ref _stepNumber, value);
        }

        private string _displayName = "";
        public string DisplayName
        {
            get => _displayName;
            set => SetField(ref _displayName, value);
        }

        private bool _isComplete;
        public bool IsComplete
        {
            get => _isComplete;
            set => SetField(ref _isComplete, value);
        }

        private string _parentName = "";
        public string ParentName
        {
            get => _parentName;
            set => SetField(ref _parentName, value);
        }

        private bool _isChecked;
        public bool IsChecked
        {
            get => _isChecked;
            set => SetField(ref _isChecked, value);
        }

       

        public LearnedStepEntity ToEntity()
        {
            return new LearnedStepEntity
            {
                LearnedStepId = this.LearnedStepId,
                Name = this.Name,
                StepNumber = this.StepNumber,
                DisplayName = this.DisplayName,
                IsComplete = this.IsComplete,
                TreasureStep = this.TreasureStep.ToString(),
                ParentName = this.ParentName
            };
        }
        public void FromEntity(LearnedStepEntity entity)
        {
            LearnedStepId = entity.LearnedStepId;
            Name = entity.Name;
            StepNumber = entity.StepNumber;
            DisplayName = entity.DisplayName;
            IsComplete = entity.IsComplete;
            ParentName = entity.ParentName;
            IsChecked = false;
        }
    }
}


