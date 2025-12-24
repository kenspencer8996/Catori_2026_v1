using CatoriesCityAppHelper_2025.Objects;
using System.ComponentModel;

namespace CatoriCity2025WPF.ViewModels
{
    public class ViewmodelBase : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;


        public void OnPropertyChanged(string propertyName)
        {
            //if (HelperStuff.Loading == false)
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
