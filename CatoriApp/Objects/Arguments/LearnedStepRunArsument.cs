using System.Collections.ObjectModel;
namespace CatoriApp.Objects.Arguments
{
    public class LearnedStepRunArgument
    {
        public ObservableCollection<LearnedStepModel> LearnedSteps { get; set; } = new();
        public string RunType { get; set; } = "All";
        public int NumberOfSpotsToRun { get; set; } = 0;
    }
}


