using CatoriCity2025WPF.Objects.Services;
using CatoriCity2025WPF.ViewModels;
using System.Collections.ObjectModel;
namespace CatoriCity2025WPF.Controllers
{
    public class BankDepositsController
    {
        private readonly DepositService _depositService;
        private readonly BankService _bamkService;
        private readonly Views.BankDepositsView _view;
        private BankViewModel _bankViewModel;
        public ObservableCollection<DepositViewModel> Deposits { get; } = new ObservableCollection<DepositViewModel>();
        decimal totalFunds = 0;

        public BankDepositsController(Views.BankDepositsView view, BankViewModel bankViewModel)
        {
            _view = view ?? throw new ArgumentNullException(nameof(view));
            _depositService = new DepositService();
            _bankViewModel = bankViewModel;
            _view.txtBankId.Text = bankViewModel.BankId.ToString();
            _view.txtBankName.Text = bankViewModel.Name.ToString();
            totalFunds = bankViewModel.CurrentFunds;
        }

        public async Task Load()
        {
            try
            {
                var list =  _depositService.GetAll();
                var filtered = from b in list
                               where b.BankId == _bankViewModel.BankId
                               select b;
        
                decimal totalDeposits = 0;
                // update observable collection on UI thread
                App.Current.Dispatcher.Invoke(() =>
                {
                    Deposits.Clear();
                    foreach (var d in filtered)
                    {
                        Deposits.Add(d);
                        totalDeposits += d.Amount;
                    }
                });
                _view.txtBankFunds.Text = (totalFunds + totalDeposits).ToString();

            }
            catch (Exception ex)
            {
                _view.SetStatus($"Error: {ex.Message}");
            }
        }
    }
}
