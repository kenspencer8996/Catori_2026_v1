using System.Collections.ObjectModel;
namespace CatoriApp.Core.Objects.Arguments
{
    public class LearnedStepRunArgument
    {
        public ObservableCollection<LearnedStepModel> LearnedSteps { get; set; } = new();
        public string RunType { get; set; } = "All";
        public int NumberOfSpotsToRun { get; set; } = 0;
    }
}


