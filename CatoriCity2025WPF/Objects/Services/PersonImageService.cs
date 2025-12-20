using CatoriCity2025WPF.ViewModels;
using CatoriCity2025WPF.Viewmodels;
using CityAppServices.Objects.database;
using CityAppServices.Objects.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace CatoriCity2025WPF.Objects.Services
{
    public class PersonImageService
    {
        PersonImageRepository repository = new PersonImageRepository();
        public async Task<List<PersonImageViewModel>> GetPersonImagesAsync()
        {
            List<PersonImageViewModel> models = new List<PersonImageViewModel>();
            List<PersonImageEntity> results = new List<PersonImageEntity>();
            try
            {
                results = await repository.GetPersonImagesAsync();
                foreach (PersonImageEntity image in results)
                {
                    PersonImageViewModel model = new PersonImageViewModel();
                    model.ToModel(image);
                    models.Add(model);
                }
            }
            catch (Exception ex)
            {
                throw;
            }
            return models;
        }
        public async Task<PersonImageViewModel> GetPersonImagebyNameAsync(string name)
        {
            PersonImageViewModel model;
            PersonImageEntity results = new PersonImageEntity();
            try
            {
                results = await repository.GetPersonImagebyNameAsync(name);
                model = new PersonImageViewModel();
                model.ToModel(results);
            }
            catch (Exception ex)
            {
                throw;
            }
            return model;
        }

        public void UpsertPersonImage(PersonImageEntity Setting)
        {
            try
            {
                repository.UpsertPersonImage(Setting);
            }
            catch (Exception ex)
            {

                throw;
            }


        }

    }
}
