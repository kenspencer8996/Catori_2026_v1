using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace CatoriApp.Core.ViewModels.Shared
{
    public class ViewmodelBase : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;

        public void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        protected bool SetProperty<T>(ref T field, T value, [CallerMemberName] string? name = null)
        {
            if (Equals(field, value))
                return false;

            field = value;
            OnPropertyChanged(name ?? string.Empty);
            return true;
        }
    }
}
