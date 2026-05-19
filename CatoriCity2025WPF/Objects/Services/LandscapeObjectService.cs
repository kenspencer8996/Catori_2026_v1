using System.Collections.ObjectModel;

namespace CatoriApp.Objects.Services
{
    internal class LandscapeObjectService
    {
        private LandscapeObjectRepository repository = new LandscapeObjectRepository();
        public async Task<ObservableCollection<LandscapeObjectViewModel>> GetLandscapeObjectsAsync(int groupid)
        {
            ObservableCollection<LandscapeObjectViewModel> LandscapeObjects = new ObservableCollection<LandscapeObjectViewModel>();
            List<LandscapeObjectEntity> landscapeObjectEntities = new List<LandscapeObjectEntity>();
            landscapeObjectEntities = await repository.GetLandscapeObjectsAsync(groupid);
            foreach (LandscapeObjectEntity landscape in landscapeObjectEntities)
            {
                LandscapeObjectViewModel landscapeObjectViewModel = new LandscapeObjectViewModel();
                landscapeObjectViewModel.EntityToModel(landscape);
                LandscapeObjects.Add(landscapeObjectViewModel);
            }   
            return LandscapeObjects;
        }
        public List<Int32> GetLandscapeObjectsGroupIds()
        {
            List<Int32> LandscapeObjectGroupIds = new List<Int32>();
            LandscapeObjectGroupIds = repository.GetLandscapeObjectsGroupIds();
            return LandscapeObjectGroupIds;
        }
        public async Task<LandscapeObjectViewModel> GetLandscapeObjectByIdAsync(Int32 LandScapeObjectID)
        {
            LandscapeObjectViewModel landscape = new LandscapeObjectViewModel();
            LandscapeObjectEntity entity = await repository.GetLandscapeObjectByIdAsync(LandScapeObjectID);
            landscape.EntityToModel(entity);
            return landscape;
        }
        public void Upsert(LandscapeObjectEntity landscape)
        {
            repository.Upsert(landscape);
        }

        internal void Delete(LandscapeObjectEntity entity)
        {
            repository.Delete(entity);
        }
    }
}

