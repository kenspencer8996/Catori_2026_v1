namespace CatoriCity2025WPF.ViewModels
{
    public class LearnedStepModel
    {
        public int FactoryInteriorId { get; set; }
        public string FactoryInteriorName { get; set; }
        public int StepNumber { get; set; }
        public string DisplayName { get; set; } = "";
        public bool IsComplete { get; set; }
        public bool IsChecked { get; set; }
        public LearnedStepEntity ToEntity()
        {
            return new LearnedStepEntity
            {
                FactoryInteriorId = this.FactoryInteriorId,
                FactoryInteriorName = this.FactoryInteriorName,
                StepNumber = this.StepNumber,
                DisplayName = this.DisplayName,
                IsComplete = this.IsComplete
            };
        }
        public void FromEntity(LearnedStepEntity entity)
        {
            FactoryInteriorId = entity.FactoryInteriorId;
            FactoryInteriorName = entity.FactoryInteriorName;
            StepNumber = entity.StepNumber;
            DisplayName = entity.DisplayName;
            IsComplete = entity.IsComplete;
            IsChecked = false;
        }
    }
}
