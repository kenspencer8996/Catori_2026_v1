
using CityAppServices.Objects.database;

namespace CatoriApp.Services
{
    public class LearnedStepService
    {
        private readonly LearnedStepRepository _repository;

        public LearnedStepService()
        {
            _repository = new LearnedStepRepository();
        }

        public LearnedStepService(LearnedStepRepository repository)
        {
            _repository = repository;
        }

        public async Task<LearnedStepModel?> GetByIdAsync(int learnedStepId)
        {
            var entity = await _repository.GetByIdAsync(learnedStepId);
            return entity != null ? MapToViewModel(entity) : null;
        }
        public List<LearnedStepModel> GetAll()
        {
            List<LearnedStepModel> learnedSteps = new List<LearnedStepModel>();
            var entities = _repository.GetAllAsync();
            foreach (var entity in entities.Result)
            {
                learnedSteps.Add(MapToViewModel(entity));
            }
            return learnedSteps;
        }
        public async Task<List<LearnedStepModel>> GetAllAsync()
        {
            var entities = await _repository.GetAllAsync();
            return entities.Select(MapToViewModel).ToList();
        }

        public async Task CreateAsync(List<LearnedStepModel> viewModels)
        {
            foreach (var viewModel in viewModels)
            {
                var entity = MapToEntity(viewModel);
                await _repository.InsertAsync(entity);
            }
        }

        public async Task UpdateAsync(List<LearnedStepModel> viewModels)
        {
            cLogger.Log("Updating learned steps...");
            foreach (var viewModel in viewModels)
            {
                var entity = MapToEntity(viewModel);
                if (entity != null && entity.LearnedStepId != 0)
                    try
                    {
                        await _repository.UpdateAsync(entity);
                    }
                    catch (Exception ex)
                    {
                        cLogger.Log("UpdateAsync... " + ex.Message);
                    }                
                else

                    try
                    {
                        await _repository.InsertAsync(entity);

                    }
                    catch (Exception ex)
                    {
                        cLogger.Log("InsertAsync exception... " + ex.Message);
                    }
            }
        }

        public async Task<int> DeleteAsync(int learnedStepId)
        {
            return await _repository.DeleteAsync(learnedStepId);
        }

        public async Task<int> MarkAsCompleteAsync(int learnedStepId)
        {
            var entity = await _repository.GetByIdAsync(learnedStepId);
            if (entity == null)
                return 0;

            entity.IsComplete =true;
            return await _repository.UpdateAsync(entity);
        }

        public async Task<int> MarkAsIncompleteAsync(int learnedStepId)
        {
            var entity = await _repository.GetByIdAsync(learnedStepId);
            if (entity == null)
                return 0;

            entity.IsComplete = false;
            return await _repository.UpdateAsync(entity);
        }

        private static LearnedStepModel MapToViewModel(LearnedStepEntity entity)
        {
            TreasureStepEnum treasureStepEnum = TreasureStepEnum.WalkToTreasureSpot;
            if (!string.IsNullOrEmpty(entity.TreasureStep) && 
                Enum.TryParse<TreasureStepEnum>(entity.TreasureStep, out var parsedEnum))
            {
                treasureStepEnum = parsedEnum;
            }

            LearnedStepModel result = new LearnedStepModel
            {
                LearnedStepId = entity.LearnedStepId,
                Name = entity.Name,
                StepNumber = entity.StepNumber,
                DisplayName = entity.DisplayName,
                IsComplete = entity.IsComplete,
                TreasureStep = treasureStepEnum,
                ParentName = entity.ParentName
            };
            return result;
        }

        private static LearnedStepEntity MapToEntity(LearnedStepModel viewModel)
        {
            return new LearnedStepEntity
            {
                LearnedStepId = viewModel.LearnedStepId,
                Name = viewModel.Name,
                StepNumber = viewModel.StepNumber,
                DisplayName = viewModel.DisplayName,
                IsComplete = viewModel.IsComplete,
                TreasureStep = viewModel.TreasureStep.ToString(),
                ParentName = viewModel.ParentName
            };
        }
    }
}
