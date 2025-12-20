using CatoriCity2025WPF.ViewModels;
using CityAppServices.Objects.database;
using CityAppServices.Objects.Entities;

namespace CatoriCity2025WPF.Objects.Services
{
    public class ImageService
    {
        ImageRepository repository = new ImageRepository();
        public async Task<List<ImageViewModel>> GetImagesAsync()
        {
            List<ImageViewModel> models = new List<ImageViewModel>();
            List<ImageDetailEntity> results = new List<ImageDetailEntity>();
            try
            {
                results = await repository.GetImagesAsync();
                foreach (var image in results)
                {
                    ImageViewModel model = new ImageViewModel();
                    model.ToModel(image); ;
                    models.Add(model);
                }

            }
            catch (Exception ex)
            {
                throw;
            }
            return models;
        }
        public async Task<ImageViewModel> GetImagebyNameAsync(string name)
        {
            ImageDetailEntity result = new ImageDetailEntity();
            ImageViewModel model = new ImageViewModel();
            try
            {
                result = await repository.GetImageByNameAsync(name);
                model.ToModel(result);
            }
            catch (Exception ex)
            {
                throw;
            }
            return model;
        }

        public void UpsertImage(ImageDetailEntity Setting)
        {
            try
            {
                repository.UpsertImage(Setting);
            }
            catch (Exception ex)
            {

                throw;
            }


        }
    }
}
