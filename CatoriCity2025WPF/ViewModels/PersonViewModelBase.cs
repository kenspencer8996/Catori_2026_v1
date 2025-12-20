using CatoriCity2025WPF.Objects;
using CatoriCity2025WPF.Objects.Arguments;
using CatoriCity2025WPF.ViewModels;
using CatoriCity2025WPF.Views.Controls;
using CityAppServices.Objects.Entities;

namespace CatoriCity2025WPF
{
    public class PersonViewModelBase:ViewmodelBase
    {
        public decimal CurrentPay { get; set; }
        HouseControl _host;
        public int _personId {  get; set; }
        public bool IsUser
        {
            get
            {
                return _isUser;
            }
            set
            {
                _isUser = value;
                OnPropertyChanged("IsUser");
            }
        }
        public void StartPay(decimal payRate)
        {
            CurrentPay = payRate;
            //_payTimer.Start();
        }
        public void LeaveWork()
        {
            CurrentPay = 0;
            //_payTimer.Stop();
        }
        public decimal Funds
        {
            get
            {
                return _Funds;
            }
            set
            {
                _Funds = value;
                OnPropertyChanged("Funds");
            }
        }

       
        private void PaytimerFired()
        {
            Funds += CurrentPay;
        }

        private void ShowTimerfired(PersonViewModel person)
        {
            _host.Visibility = System.Windows.Visibility.Visible;
            PersonTimerFiredEventArg ev = new PersonTimerFiredEventArg(person);
            ShowPersonTimerFired(_host, ev);
        }

        private void ShowPersonTimerFired(HouseControl host, PersonTimerFiredEventArg ev)
        {
        }

        private string _name = "";

        private bool _isUser = false;
        private decimal _Funds = 0;
        public object Host { get; set; }
        public Type HostType { get; set; }
        private PersonEnum _PersonRole = PersonEnum.Individual;
        private int _TimerSeconds = 49;
        private bool _active = false;
        private string _imagesFolder = ""; 
        public string ImagesFolder
        {
            get
            {
                return _imagesFolder;
            }
            set
            {
                _imagesFolder = value;
            }
        }
        public string Name
        {
            get
            {
                return _name;
            }
            set
            {
                _name = value;
            }
        }
        public bool Active
        {
            get
            {
                return _active;
            }
            set
            {
                _active = value;
            }
        }
      
        public PersonEnum PersonType { get; set; }
        public bool Girl { get; set; }
        public void Add(string name, bool girl, PersonEnum personType)
        {
            Name = name;
            PersonType = personType;
            Girl = girl;
        }
        public List<PersonViewModel> Friends { get; set; }
    
        public PersonEntity GetEntity()
        {
            PersonEntity person = new PersonEntity();
            person.PersonId = _personId;
            person.PersonRole = _PersonRole;
            person.Active = _active;
            person.ImagesFolder = _imagesFolder;
            person.IsUser = _isUser;
            person.Name = _name;
            return person;
        }
        public void ToModel(PersonEntity person)
        {
            _personId = person.PersonId;
            _PersonRole = person.PersonRole;
            _active= person.Active;
            _imagesFolder = person.ImagesFolder;
            _isUser = person.IsUser;
            _name = person.Name;
        }
    }
}
