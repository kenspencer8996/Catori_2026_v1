using CatoriCity2025WPF.Controllers;
using CatoriCity2025WPF.Objects;
using CatoriCity2025WPF.Objects.Arguments;
using CatoriCity2025WPF.ViewModels;
using CityAppServices;
using CityAppServices.Objects.Entities;
using CommunityToolkit.Mvvm.Messaging;
using System.Windows;
using System.Windows.Controls;

namespace CatoriCity2025WPF.Views
{
    /// <summary>
    /// Interaction logic for SettingsView.xaml
    /// </summary>
    public partial class SettingsView : Window
    {
        SettingsViewController _controller;
        public event EventHandler<ReloadLandscapeSettingsArg> OnLandscapeGroupChange;
        public bool IsDirty { get; set; } = false;
        public SettingsView()
        {
            InitializeComponent();

            _controller = new SettingsViewController(this);
            Settingsgrid.ItemsSource = GlobalServices.Settings;
            LoadLandscapeGroups(GlobalServices.LandscapeObjecGroupid);
            SetControls();
            ScreenBackgroundColorPicker.OnColorChanged += ScreenBackgroundColorPicker_OnColorChanged; ;
        }

        private void ScreenBackgroundColorPicker_OnColorChanged(object? sender, ColorChangedArgs e)
        {
            ScreenBackgroundColor = e.ColorName;
        }

        internal Button GetButton()
        {
            Button landscapeObjectButton = new Button();
            landscapeObjectButton.Width = 25;
            //Style buttonstyle =(Style)Application.Current.Resources["ButtonFocusVisual"];
            //landscapeObjectButton.Style = buttonstyle;
            
            landscapeObjectButton.Foreground = System.Windows.Media.Brushes.Red;
            landscapeObjectButton.FontWeight = FontWeights.Bold;
            landscapeObjectButton.Click += LandscapeObjectButton_Click;
            return landscapeObjectButton;
        }
        internal void LandscapeObjectButton_Click(object sender, RoutedEventArgs e)
        {
            Button button = (Button)sender;
            string groupidstring = button.Content.ToString();
            Int32 groupid = Convert.ToInt32(groupidstring);
            GroupsLabel.Content = _controller.GroupLabelText + " " + groupidstring;

            LoadLandscapeGroups(groupid);
            if (OnLandscapeGroupChange != null)
            {
                ReloadLandscapeSettingsArg arg = new ReloadLandscapeSettingsArg();
                OnLandscapeGroupChange(this, arg);
            }
        }
        private void LoadLandscapeGroups(Int32 groupid)
        {
            GlobalServices.LandscapeObjecGroupid = groupid;
            LandscapeGroups.ItemsSource = GlobalStuff.LandscapeObjects;

        }

        private void SaveSettingsButton_Click(object sender, RoutedEventArgs e)
        {
            _controller.SaveSettings();
        }
      

        internal void SetControls()
        {
            //BadPersonWidthTextBox.Text = GlobalStuff.BadPersonWidth.ToString();
            //BadPersonHeightTextBox.Text = GlobalStuff.BadPersonHeight.ToString();
            //PoliceCarSpeedTextBox.Text = GlobalStuff.PoliceCarSpeed.ToString();
            //BadGuyTravelSpeedTextBox.Text = GlobalStuff.BadGuyTravelSpeed.ToString();
            //EmailLogTextBox.Text = GlobalStuff.EmailLog.ToString();
            //campfireanimationspeedTextBox.Text = GlobalStuff.CampFireAnimationSpeed.ToString();
            //BadguyCountTextBox.Text = GlobalStuff.BadguyCount.ToString();
            //CampfireSleeptimeTextBox.Text = GlobalStuff.CampfireSleeptime.ToString();
        }

        private void LandscapeGroups_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var dataGrid = sender as DataGrid;
            var selectedItem = dataGrid.SelectedItem;
            if (selectedItem != null)
            {
                IsDirty = true;
                SelectFilename((LandscapeObjectViewModel)selectedItem);
            }
        }

        private void AddNewLandscapeItem_Click(object sender, RoutedEventArgs e)
        {
            IsDirty = true;
            LandscapeObjectViewModel modlel = new LandscapeObjectViewModel();
            SelectFilename(modlel);
        }
        private void SelectFilename(LandscapeObjectViewModel model)
        {
            IsDirty = true;
            LandscapeObjectDetailView view = new LandscapeObjectDetailView(model);
            view.Owner = this;
            view.ShowDialog();
        }

        private void Settingsgrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var dataGrid = sender as DataGrid;
            SettingEntity selectedItem =(SettingEntity)dataGrid.SelectedItem;
            SetingDetailView view = new SetingDetailView(selectedItem);
            view.Owner = this;
            view.ShowDialog();
        }

        private void AddNewSetting_Click(object sender, RoutedEventArgs e)
        {
            IsDirty = true;
            SettingEntity selectedItem = new SettingEntity();
            SetingDetailView view = new SetingDetailView(selectedItem);
            view.Owner = this;
            view.ShowDialog();

        }
        #region Color events and properties
        public string ScreenBackgroundColor { get; set; } 
      

        #endregion

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (IsDirty)
            {
                MessageBoxResult result = MessageBox.Show("Do you want to save your setting changes?", "Save Changes", MessageBoxButton.YesNoCancel);
                if (result == MessageBoxResult.Yes)
                {
                    _controller.SaveSettings();
                }
                else if (result == MessageBoxResult.Cancel)
                {
                    e.Cancel = true; // Cancel the closing event
                }
            }
        }

        private void PoliceCarsButton_Click(object sender, RoutedEventArgs e)
        {
            this.Hide();
            GlobalStuff.MainView.RunPoliceCars();
        }

        private void BadPersonButton_Click(object sender, RoutedEventArgs e)
        {
            this.Hide();
            FundsViewModel funds = new FundsViewModel();
            funds.Money = 500;
            funds.X = 150;
            funds.Y = GlobalStuff.IntersectuonMikMoo.y;

            FundsMessage msg = new FundsMessage(funds);
            WeakReferenceMessenger.Default.Send(msg);
        }

        private void BanksButton_Click(object sender, RoutedEventArgs e)
        {
            BankListView bankListView = new BankListView();
            bankListView.Owner = this;
            bankListView.ShowDialog();
        }
    }
}
