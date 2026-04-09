namespace CatoriCity2025WPF.Objects.Services
{
    public class LearnedStepService
    {
        private readonly LearnedStepRepository _repo;

        public LearnedStepService()
        {
            _repo = new LearnedStepRepository();
        }

        public void Create(LearnedStepEntity step) => _repo.Insert(step);

        public void Update(LearnedStepEntity step) => _repo.Update(step);

        public void UpdateStepCompletion(int factoryInteriorId, int stepNumber, bool isComplete)
        {
            _repo.UpdateIsComplete(factoryInteriorId, stepNumber, isComplete);
        }

        public void Delete(int factoryInteriorId, int stepNumber) => _repo.Delete(factoryInteriorId, stepNumber);

        public void DeleteAllByFactoryInterior(int factoryInteriorId) => _repo.DeleteAllByFactoryInteriorId(factoryInteriorId);

        public LearnedStepEntity? GetStep(int factoryInteriorId, int stepNumber)
        {
            return _repo.GetByFactoryInteriorIdAndStepNumber(factoryInteriorId, stepNumber);
        }

        public List<LearnedStepEntity> GetAllSteps()
        {
            return _repo.GetAll();
        }

        /// <summary>
        /// Populates the TreasureFieldLearnRunStepsviewModel with steps for the specified factory interior
        /// </summary>
        public TreasureFieldLearnRunStepsviewModel LoadStepsIntoViewModel(TreasureFieldLearnRunStepsviewModel viewModel, string factoryInteriorname)
        {
            viewModel.LearnedSteps.Clear();
            
            var steps = _repo.GetByFactoryInteriorFactoryName(factoryInteriorname);
            
            foreach (var step in steps)
            {
                LearnedStepModel model = new LearnedStepModel();
                model.FromEntity(step);

                viewModel.LearnedSteps.Add(model);
            }
            return viewModel;
        }

        /// <summary>
        /// Converts entity list to viewmodel
        /// </summary>
        public TreasureFieldLearnRunStepsviewModel ToViewModel(List<LearnedStepEntity> steps)
        {
            var viewModel = new TreasureFieldLearnRunStepsviewModel();
            
            foreach (var step in steps)
            {
                LearnedStepModel model = new LearnedStepModel();
                model.FromEntity(step);

                viewModel.LearnedSteps.Add(model);
            }
            
            return viewModel;
        }

        /// <summary>
        /// Converts viewmodel back to entity list
        /// </summary>
        public List<LearnedStepModel> FromViewModel(TreasureFieldLearnRunStepsviewModel viewModel)
        {
            return viewModel.LearnedSteps.ToList();
        }

        /// <summary>
        /// Saves all steps from the viewmodel to the database
        /// </summary>
        public void SaveViewModelSteps(TreasureFieldLearnRunStepsviewModel viewModel)
        {
            foreach (var step in viewModel.LearnedSteps)
            {
                var existing = _repo.GetByFactoryInteriorIdAndStepNumber(step.FactoryInteriorId, step.StepNumber);
                LearnedStepEntity entity = new LearnedStepEntity();
                 entity = step.ToEntity();
                if (existing != null)
                {
                    _repo.Update(entity);
                }
                else
                {
                    _repo.Insert(entity);
                }
            }
        }
    }
}
