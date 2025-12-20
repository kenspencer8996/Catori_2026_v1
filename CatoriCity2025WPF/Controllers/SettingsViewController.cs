using CatoriCity2025WPF.Objects;
using CatoriCity2025WPF.Objects.Services;
using CatoriCity2025WPF.Views;
using CityAppServices;
using CityAppServices.Objects.Entities;
using System.Windows;
using System.Windows.Controls;

namespace CatoriCity2025WPF.Controllers
{
    internal class SettingsViewController
    {
        SettingsView _view;
        public string GroupLabelText = "Group";
        LandscapeObjectService landscapeservice = new LandscapeObjectService();
        SettingService settingService = new SettingService();

        internal SettingsViewController(SettingsView view)
        {
            _view = view;
            LoadGroupList();
        }
        private void LoadGroupList()
        {
            foreach (var landscapeObject in GlobalStuff.landscapeObjectGroupIds)
            {
                Button landscapeObjectButton = _view.GetButton();


                if (GlobalServices.LandscapeObjecGroupid == landscapeObject)
                {
                    //    landscapeObjectButton.IsChecked = true;
                    _view.GroupsLabel.Content = GroupLabelText + " " + landscapeObject.ToString();
                }

                landscapeObjectButton.Foreground = System.Windows.Media.Brushes.Red;
                landscapeObjectButton.Content = landscapeObject.ToString();
                landscapeObjectButton.FontWeight = FontWeights.Bold;
                landscapeObjectButton.Click += LandscapeObjectButton_Click;
                landscapeObjectButton.Tag = landscapeObject.ToString();
                landscapeObjectButton.ToolTip = "Click to load this landscape group " + landscapeObject.ToString();
                _view.GroupsIdsList.Children.Add(landscapeObjectButton);
            }
        }

        private void LandscapeObjectButton_Click(object sender, RoutedEventArgs e)
        {
        }

        public void SaveSettings()
        {
            foreach (var item in GlobalServices.Settings)
            {
                settingService.UpsertSetting(item);
            }
        }
      
    }
}
