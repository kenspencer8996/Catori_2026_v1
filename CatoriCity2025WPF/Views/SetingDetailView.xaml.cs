using CityAppServices.Objects.Entities;
using System.Windows;

namespace CatoriCity2025WPF.Views
{
    /// <summary>
    /// Interaction logic for SetingDetailView.xaml
    /// </summary>
    public partial class SetingDetailView : Window
    {
        SettingEntity _settingEntity;
        public SetingDetailView(SettingEntity settingEntity)
        {
            InitializeComponent();
            _settingEntity = settingEntity;
            DataContext = _settingEntity;
        }
    }
}
