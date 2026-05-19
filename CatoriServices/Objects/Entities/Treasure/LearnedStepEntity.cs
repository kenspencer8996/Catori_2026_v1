namespace CatoriServices.Objects.Entities.Treasure
{
    public class LearnedStepEntity
    {
        private string _treasureStep;
        public int LearnedStepId { get; set; }
        public string Name { get; set; } = string.Empty;
        public int StepNumber { get; set; }
        public string DisplayName { get; set; } = string.Empty;
        public bool IsComplete { get; set; }
        public string TreasureStep 
        { 
            get
            {
                return _treasureStep;
            }
            set
            {
                _treasureStep = value;
            }
        } 
        public string ParentName { get; set; } = string.Empty;

    }
}

