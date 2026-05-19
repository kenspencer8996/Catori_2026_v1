using CityAppServices.Objects.Entities;
using System.Windows;

namespace CatoriApp.Views
{
    /// <summary>
    /// Interaction logic for SettingDetailView.xaml
    /// </summary>
    public partial class SettingDetailView : Window
    {
        SettingEntity _settingEntity;
        public SettingDetailView(SettingEntity settingEntity)
        {
            InitializeComponent();
            _settingEntity = settingEntity;
            DataContext = _settingEntity;
        }
    }
}

