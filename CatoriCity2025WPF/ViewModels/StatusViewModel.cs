namespace CatoriCity2025WPF.ViewModels
{
    public class StatusViewModel:ViewmodelBase
    {
        private string _statusMessage;
        public string StatusMessage
        {
            get => _statusMessage;
            set
            {
                if (_statusMessage != value)
                {
                    _statusMessage = value;
                    OnPropertyChanged(StatusMessage);
                }
            }
        }
        private string _name = string.Empty;
        public string Name
        {
            get => _name;
            set
            {
                if (_name != value)
                {
                    _name = value;
                    OnPropertyChanged(nameof(Name));
                }
            }
        }

        private decimal _funds = 0m;
        public decimal Funds
        {
            get => _funds;
            set
            {
                if (_funds != value)
                {
                    _funds = value;
                    OnPropertyChanged(nameof(Funds));
                }
            }
        }
    }
}
