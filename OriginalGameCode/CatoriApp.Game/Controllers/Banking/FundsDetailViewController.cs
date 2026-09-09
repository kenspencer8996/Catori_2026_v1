using CatoriApp.Core.Objects.Arguments;
using CatoriApp.Game.Views;
using CommunityToolkit.Mvvm.Messaging;
namespace CatoriApp.Game.Controllers.Banking
{
    public class FundsDetailViewController
    {
        FundsDetailView _view;
        FundsViewModel _fundsViewModel;
        DepositViewModel _depositViewModel;
        List<DepositViewModel> _depositscurrent;
        PersonViewModel _personViewModel;
        PersonService _personService;
        private BankService _bankService;
        private BankViewModel? _selectedBank;

        int _personId;
        decimal _depositAmount;
        DepositService depositservice;
        private SynchronizationContext _uiContext;


        public FundsDetailViewController(FundsDetailView view)
        {
            _view = view;
            _personViewModel = GlobalGame.CurrentPerson;
            _personId = _personViewModel.PersonId;
            _bankService = new BankService();
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
            var banks = _bankService.GetAllAsync();
            _view.BankStackComboBox.SelectedValuePath = "Name";
            _view.BankStackComboBox.DisplayMemberPath = "Name";
            _view.BankStackComboBox.ItemsSource =banks.Result;

      
            if (_depositViewModel != null)
            {
                _view.BankStackComboBox.SelectedValue = _depositViewModel.BankId;
            }
            WeakReferenceMessenger.Default.Register<MessageSaveDepositArgument>(this, (r, m) =>
            {
                LoadDepositsForPerson(_personId);
                var banks = CityScapeGlobal.Banks;
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

        public async void SendDepositToBank()
        {
            if (_depositViewModel != null)
            {
                try
                {
                    var service=new CatoriServices.Objects.Services.Finance.FinanceService();
                    var result=await Task.Run(()=>service.DepositToBank(_personId,_depositViewModel.BankId,
                        _depositAmount,_depositViewModel.BusinessName));
                    _personViewModel.Funds=CatoriServices.Objects.Services.Finance.FinanceService.FromCents(result.WalletBalanceCents);
                    _fundsViewModel.Money=_personViewModel.Funds;
                    _depositAmount=0;
                    _view.DepositAmountTextBox.Text="0";
                    _view.MessageLabel.Content="Deposit completed.";
                    LoadDepositsForPerson(_personId);
                    WeakReferenceMessenger.Default.Send(new MessageSaveDepositArgument());
                }
                catch(Exception exception)
                {
                    _view.MessageLabel.Content=exception.Message;
                }
            }
        }
        
        internal void BankSelected(BankViewModel selectedItem)
        {
            if (selectedItem == null) return;
            _selectedBank = selectedItem;
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

        internal void FileBankruptcy()
        {
            try
            {
                var service = new CatoriServices.Objects.Services.Finance.FinanceService();
                service.FileBankruptcy(_personId, "Filed by player at ATM.");
                _view.MessageLabel.Content = "Bankruptcy filed. You may now request a recovery loan.";
            }
            catch (Exception exception)
            {
                _view.MessageLabel.Content = exception.Message;
            }
        }

        internal void RequestRecoveryLoan()
        {
            if (_selectedBank == null)
            {
                _view.MessageLabel.Content = "Select a bank first.";
                return;
            }
            if (_depositAmount <= 0)
            {
                _view.MessageLabel.Content = "Enter the requested loan amount in Amount.";
                return;
            }

            try
            {
                var service = new CatoriServices.Objects.Services.Finance.FinanceService();
                var result = service.IssueBankruptcyLoan(_personId, _selectedBank.BankId,
                    _depositAmount, _selectedBank.InterestRate);
                _personViewModel.Funds = CatoriServices.Objects.Services.Finance.FinanceService.FromCents(result.WalletBalanceCents);
                _fundsViewModel.Money = _personViewModel.Funds;
                _depositAmount = 0;
                _view.DepositAmountTextBox.Text = "0";
                _view.MessageLabel.Content = "Recovery loan approved and added to your wallet.";
            }
            catch (Exception exception)
            {
                _view.MessageLabel.Content = exception.Message;
            }
        }

        internal void DepositAmountChanged()
        {
            _depositAmount = Convert.ToDecimal( _view.DepositAmountTextBox.Text);
            if (_depositAmount > 0 && _depositAmount <= _personViewModel.Funds)
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


