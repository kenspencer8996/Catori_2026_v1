using System.Collections.ObjectModel;

namespace CatoriCity2025WPF.Objects.Arguments
{
    public class LearnedStepRunArgument
    {
        public ObservableCollection<LearnedStepModel> LearnedSteps { get; set; } = new();
        public string RunType { get; set; } = "All";
        public int NumberOfSpotsToRun { get; set; } = 0;
    }
}
