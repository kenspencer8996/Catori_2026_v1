namespace CatoriServices.Objects.Entities
{
    public class LearnedStepEntity
    {
        public int FactoryInteriorId { get; set; }
        public string FactoryInteriorName { get; set; }
        public int StepNumber { get; set; }
        public string DisplayName { get; set; } = "";
        public bool IsComplete { get; set; }
        public bool IsChecked { get; set; }
    }
}
