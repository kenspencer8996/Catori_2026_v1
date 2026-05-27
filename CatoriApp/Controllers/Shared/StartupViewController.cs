using CatoriApp.Views.Shared;
namespace CatoriApp.Controllers.Shared
{
    internal class StartupViewController
    {
        StartupView _view;
        public StartupViewController(StartupView view)
        {
            _view = view;
            LoadPersons();
        }

        private void LoadPersons()
        {
            PersonService personService = new PersonService();
            GlobalAllApps.AllPersons = personService.GetPersonsAsync().Result;

            GlobalAllApps.CurrentPerson = GetCurrentPersonModel();

        }
        private PersonViewModel GetCurrentPersonModel()
        {
            PersonViewModel currentPerson = new PersonViewModel();
            

                var person = from i in GlobalAllApps.AllPersons
                             where i.Name == "Catori"
                             select i;
                currentPerson = person.First();
            
            return currentPerson;
        }
    }
}

