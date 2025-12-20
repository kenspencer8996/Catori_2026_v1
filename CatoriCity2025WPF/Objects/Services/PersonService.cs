using CatoriCity2025WPF.Viewmodels;
using CityAppServices.Objects.database;
using CityAppServices.Objects.Entities;
using System.Xml.Linq;

namespace CatoriCity2025WPF.Objects.Services
{
    public class PersonService
    {
        PersonRepository repository = new PersonRepository();
        public async Task<List<PersonViewModel>> GetPersonsAsync()
        {
            List<PersonViewModel> results = new List<PersonViewModel>();
            try
            {
                List<PersonEntity> persons;
                persons = await repository.GetPersonsAsync();
                foreach (var item in persons)
                {
                    PersonViewModel pvm = new PersonViewModel();
                    pvm.ToModel(item);
                    results.Add(pvm);
                }
            }
            catch (Exception ex)
            {
                throw;
            }
            return results;
        }
        public async Task<PersonViewModel> GetPersonbyNameAsync(string name)
        {
            PersonViewModel result = new PersonViewModel();
            try
            {
                PersonEntity person;
                person = await repository.GetPersonbyNameAsync(name);
                result.ToModel(person);
            }
            catch (Exception ex)
            {
                throw;
            }
            return result;
        }
        public async Task<PersonViewModel> GetPersonbyIdAsync(int Id)
        {
            PersonViewModel result = new PersonViewModel();
            try
            {
                PersonEntity person;
                person = await repository.GetPersonbyIdAsync(Id);
                result.ToModel(person);
            }
            catch (Exception ex)
            {
                throw;
            }
            return result;
        }
        public void UpsertPerson(PersonViewModel pvm)
        {
            PersonEntity personEntity = pvm.GetEntity();
            try
            {
                repository.UpsertPerson(personEntity);
            }
            catch (Exception ex)
            {

                throw;
            }


        }
    }
}
