using CatoriCity2025WPF.Objects;
using CatoriCity2025WPF.Objects.Arguments;
using CatoriCity2025WPF.Objects.Services;
using CatoriCity2025WPF.Viewmodels;
using CatoriCity2025WPF.ViewModels;
using CatoriCity2025WPF.Views;
using CatoriServices.Objects.Entities;
using CommunityToolkit.Mvvm.Messaging;
using System.Net.WebSockets;

namespace CatoriCity2025WPF.Convertors
{
    public class FundsDetailViewController
    {
        FundsDetailView _view;
        FundsViewModel _fundsViewModel;
        DepositViewModel _depositViewModel;
        List<DepositViewModel> _depositscurrent;
        PersonViewModel _personViewModel;
        PersonService _personService;
        int _personId;
        decimal _depositAmount;
        DepositService depositservice;
        private SynchronizationContext _uiContext;


        public FundsDetailViewController(FundsDetailView view, PersonViewModel person)
        {
            _view = view;
            _personId = person.PersonId;
            _personViewModel = person;
            _view.DepositButton.IsEnabled = false;
            depositservice = new DepositService();
            _fundsViewModel = new FundsViewModel
            {
                Money = _personViewModel.Funds,
            };
            _uiContext = SynchronizationContext.Current; // Capture UI context

            _view.DataContext = _fundsViewModel;
            depositservice = new DepositService();
            LoadDepositsForPerson(_personId);
            var banks = GlobalStuff.Banks;
            _view.BankStackComboBox.SelectedValuePath = "Name";
            _view.BankStackComboBox.DisplayMemberPath = "Name";
            _view.BankStackComboBox.ItemsSource =banks;

      
            if (_depositViewModel != null)
            {
                _view.BankStackComboBox.SelectedValue = _depositViewModel.BankId;
            }
            WeakReferenceMessenger.Default.Register<MessageSaveDepositArgument>(this, (r, m) =>
            {
                LoadDepositsForPerson(_personId);
                var banks = GlobalStuff.Banks;
            });
        }

        private void CalculateTotalDeposits()
        {
            decimal totalFunds = 0;
            if (_depositscurrent == null) return;
            foreach (var item in _depositscurrent)
            {
                totalFunds += item.Amount;
            }
            _view.TotalAmount.Content = $"Total Funds: {totalFunds}";
        }

        public void SendDepositToBank()
        {
            if (_depositViewModel != null)
            {
                _depositViewModel.BankId = _depositViewModel.BankId;
                //todoL send
                DepositMessageArgument depositMessageArgument = new DepositMessageArgument
                {
                    BankId = _depositViewModel.BankId,
                    Amount = _depositAmount,
                    Person = _personViewModel,
                    BusinessName = _depositViewModel.BusinessName,
                };
                WeakReferenceMessenger.Default.Send(depositMessageArgument);
            }
        }
        
        internal void BankSelected(BankViewModel selectedItem)
        {
            _depositViewModel = new DepositViewModel();
            if (_depositViewModel == null)
            {
                _depositViewModel = new DepositViewModel
                {
                    PersonId = _personId,
                    BankId = selectedItem.BankId,
                    BusinessName = selectedItem.Name
                };
            }
            else
            {
                _depositViewModel.BankId = selectedItem.BankId;
                _depositViewModel.PersonId = _personId;
                _depositViewModel.BusinessName = selectedItem.Name;
            }
            var founddeposit = from d in _depositscurrent where d.BankId == selectedItem.BankId select d;
            if (founddeposit.Any())
            {
                _depositViewModel = founddeposit.First();
            }
        }

        internal void DepositAmountChanged()
        {
            _depositAmount = Convert.ToDecimal( _view.DepositAmountTextBox.Text);
            if (_depositAmount > 0 && _depositAmount < _personViewModel.Funds)
            {
                _view.MessageLabel.Content = "You can make this deposit.";
                _view.DepositButton.IsEnabled = true;
            }
            else
            {
                _view.MessageLabel.Content = "You do not have sufficient funds to make this deposit.";
                _view.DepositButton.IsEnabled = false;
            }
        }

        internal void DepositFunds()
        {
            //save
            //sendtobank
        }
        
        private void LoadDepositsForPerson(int personId)
        {
            // Implementation to load deposits for a specific person
            Task task3 = Task.Run(() =>
            {
                _depositscurrent = depositservice.GetDepositsForPersonAsync(personId);
            })
            .ContinueWith(t =>
            {
                _uiContext.Post(state =>
                {
                    _view.TransactionsListBox.ItemsSource = _depositscurrent;
                    CalculateTotalDeposits();
                }, null);

            });
        }   
    }
}
