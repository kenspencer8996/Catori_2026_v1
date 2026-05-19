using System.ComponentModel;
using System.Runtime.CompilerServices;
namespace CatoriApp.Views.Controls.CommonControls
{
    public class LedLight : INotifyPropertyChanged
    {
        private bool _isOn;
        private Brush _onBrush = Brushes.LimeGreen;
        private Brush _offBrush = new SolidColorBrush(Color.FromRgb(40, 40, 40));

        public bool IsOn
        {
            get => _isOn;
            set { if (_isOn != value) { _isOn = value; OnPropertyChanged(); OnPropertyChanged(nameof(CurrentBrush)); } }
        }

        public Brush OnBrush
        {
            get => _onBrush;
            set { if (!Equals(_onBrush, value)) { _onBrush = value; OnPropertyChanged(); OnPropertyChanged(nameof(CurrentBrush)); } }
        }

        public Brush OffBrush
        {
            get => _offBrush;
            set { if (!Equals(_offBrush, value)) { _offBrush = value; OnPropertyChanged(); OnPropertyChanged(nameof(CurrentBrush)); } }
        }

        public Brush CurrentBrush => IsOn ? OnBrush : OffBrush;

        public event PropertyChangedEventHandler? PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string? name = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}

