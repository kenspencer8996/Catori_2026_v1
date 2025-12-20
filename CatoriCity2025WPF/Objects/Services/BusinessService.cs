using CatoriCity2025WPF.Viewmodels;
using CityAppServices.Objects.database;
using CityAppServices.Objects.Entities;

namespace CatoriCity2025WPF.Objects.Services
{
    public class BusinessService
    {
        BusinessRepository repository = new BusinessRepository();
        public async Task<List<BusinessViewModel>> GetBusinesssAsync()
        {
            List<BusinessEntity> results = new List<BusinessEntity>();
            List<BusinessViewModel> models = new List<BusinessViewModel>();
            try
            {
                results = await repository.GetBusinesssAsync();
                foreach (BusinessEntity business in results)
                {
                    BusinessViewModel model = new BusinessViewModel();
                    model.ToModel(business); 
                    models.Add(model);
                }
            }
            catch (Exception ex)
            {
                throw;
            }
            return models;
        }
        public async Task<BusinessViewModel> GetBusinessbyNameAsync(string name)
        {
            BusinessViewModel Settingout = new BusinessViewModel();
            BusinessEntity results = new BusinessEntity();
            BusinessViewModel businessViewModel = new BusinessViewModel();
            try
            {
                results = await repository.GetBusinessbyNameAsync(name);
                businessViewModel.ToModel(results);
            }
            catch (Exception ex)
            {
                throw;
            }
            return businessViewModel;
        }

        public void UpsertBusiness(BusinessViewModel Setting)
        {
            try
            {
                repository.UpsertBusiness(Setting.GetEntity());
            }
            catch (Exception ex)
            {

                throw;
            }
        }

    }
}

