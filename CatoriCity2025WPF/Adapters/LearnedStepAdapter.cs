using CatoriServices.Objects.Entities;
using CatoriCity2025WPF.ViewModels;
using System.Collections.Generic;
using System.Linq;

namespace CatoriCity2025WPF.Adapters
{
    public static class LearnedStepAdapter
    {
        /// <summary>
        /// Converts LearnedStepEntity to LearnedStepModel
        /// </summary>
        public static LearnedStepModel ToModel(this LearnedStepEntity entity)
        {
            return new LearnedStepModel
            {
                FactoryInteriorId = entity.FactoryInteriorId,
                FactoryInteriorName = entity.FactoryInteriorName,
                StepNumber = entity.StepNumber,
                DisplayName = entity.DisplayName,
                IsComplete = entity.IsComplete,
                IsChecked = false
            };
        }

        /// <summary>
        /// Converts LearnedStepModel to LearnedStepEntity
        /// </summary>
        public static LearnedStepEntity ToEntity(this LearnedStepModel model)
        {
            return new LearnedStepEntity
            {
                FactoryInteriorId = model.FactoryInteriorId,
                FactoryInteriorName = model.FactoryInteriorName,
                StepNumber = model.StepNumber,
                DisplayName = model.DisplayName,
                IsComplete = model.IsComplete
            };
        }

        /// <summary>
        /// Converts a list of LearnedStepEntity to LearnedStepModel
        /// </summary>
        public static List<LearnedStepModel> ToModelList(this IEnumerable<LearnedStepEntity> entities)
        {
            return entities.Select(e => e.ToModel()).ToList();
        }

        /// <summary>
        /// Converts a list of LearnedStepModel to LearnedStepEntity
        /// </summary>
        public static List<LearnedStepEntity> ToEntityList(this IEnumerable<LearnedStepModel> models)
        {
            return models.Select(m => m.ToEntity()).ToList();
        }

        /// <summary>
        /// Updates an existing LearnedStepEntity with values from LearnedStepModel
        /// </summary>
        public static void UpdateFromModel(this LearnedStepEntity entity, LearnedStepModel model)
        {
            entity.FactoryInteriorId = model.FactoryInteriorId;
            entity.FactoryInteriorName = model.FactoryInteriorName;
            entity.StepNumber = model.StepNumber;
            entity.DisplayName = model.DisplayName;
            entity.IsComplete = model.IsComplete;
        }

        /// <summary>
        /// Updates an existing LearnedStepModel with values from LearnedStepEntity
        /// </summary>
        public static void UpdateFromEntity(this LearnedStepModel model, LearnedStepEntity entity)
        {
            model.FactoryInteriorId = entity.FactoryInteriorId;
            model.FactoryInteriorName = entity.FactoryInteriorName;
            model.StepNumber = entity.StepNumber;
            model.DisplayName = entity.DisplayName;
            model.IsComplete = entity.IsComplete;
        }
    }
}
