using CatoriCity2025WPF.Objects;
using CityAppServices;
using System.ComponentModel;
using System.Xml.Linq;

namespace CatoriCity2025WPF.ViewModels
{
    public class MainWindowViewModel: INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        private double _policecartravelSpeed;
        private double _badguytravelSpeed ;

        public double PolicecarToravelSpeed
        {
            get => _policecartravelSpeed;
            set
            {
                if (_policecartravelSpeed != value)
                {
                    _policecartravelSpeed = value;
                    var policecartravelSpeedEntity = GlobalServices.GettingSetting("PoliceCarTravelSpeed");
                    policecartravelSpeedEntity.IntSetting = (int)_policecartravelSpeed;

                    OnPropertyChanged(nameof(PolicecarToravelSpeed));
                }
            }
        }
        public double BadGuyTravelSpeed
        {
            get => _badguytravelSpeed;
            set
            {
                if (_badguytravelSpeed != value)
                {
                    _badguytravelSpeed = value;
                    var badguytravelspeed = GlobalServices.GettingSetting("BadGuyTravelSpeed");
                    badguytravelspeed.IntSetting = (int)_badguytravelSpeed;

                    OnPropertyChanged(nameof(BadGuyTravelSpeed));
                }
            }
        }
    }


    
}
