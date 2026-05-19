
using CatoriApp.Views.Controls.House;
using System.Net.NetworkInformation;
using System.Windows.Input;

namespace CatoriApp.Views
{
    /// <summary>
    /// Interaction logic for DetailHouseInsideView.xaml
    /// </summary>
    public partial class DetailHouseInsideView : Window
    {
        HouseViewModel _houseViewModel;
        private string _houseLivingRoomImagePath;
        private string _garageImagePath;
        double thisWith;
        double thisHeight;
        HardwareItemsControl hardwareItems;
        public DetailHouseInsideView(HouseViewModel houseViewModel,double width,double height)
        {
            InitializeComponent();
            _houseViewModel = houseViewModel;
            this.Width = width;
            this.Height = height + 100;
            double shrinkLeft= this.Width - 30;
            Canvas.SetLeft(ShrinkButton, shrinkLeft);
            thisWith = this.Width;
            thisHeight = this.Height;
            HouseService houseService = new HouseService();
            _houseViewModel = houseService.GetHouseById(_houseViewModel.HouseId);
            _houseLivingRoomImagePath = _houseViewModel.ImageLivingRoomFileName;
            _garageImagePath = _houseViewModel.ImageGarageFileName;
            MainImage.Source =UIUtility.GetImageControl( _houseLivingRoomImagePath,thisHeight,thisWith,1).Source;
            MainImage.Width = Width;
            MainImage.Height = Height;
            double buttonloc = this.Width;
            buttonloc = buttonloc - 30;
            Canvas.SetLeft(ShrinkButton, buttonloc);
            DataContext = _houseViewModel;

            EntryDoorToGarage.Width = 30;
            EntryDoorToGarage.Height = 60;
            EntryDoorToGarage.OnOpenDoor += EntryDoorToGarage_OnOpenDoor;


            if (houseViewModel.Name.Trim().ToLower() == CityScapeGlobal.CurrentHouseName.Trim().ToLower())
            {
                LoadProducts();
            }
        }

        private void LoadProducts()
        {
            PersonProductsOwnedService service = new PersonProductsOwnedService();
            var products = service.GetByPersonIdWithShopItemDetailsAsync(GlobalAllApps.CurrentPerson.PersonId).Result;
            hardwareItems = new HardwareItemsControl(products);
            MainLayoutDetailHome.Children.Add(hardwareItems);
            hardwareItems.Visibility = Visibility.Hidden;
        }

        private void EntryDoorToGarage_OnOpenDoor(object? sender, Objects.Arguments.DoorOpenEventArgs e)
        {
            MainImage.Source = UIUtility.GetImageControl(_garageImagePath, thisHeight, thisWith, 1).Source;
            EntryDoorToGarage.Visibility = Visibility.Hidden;
            Canvas.SetLeft(hardwareItems, _houseViewModel.GarageProductsLocX);
            Canvas.SetTop(hardwareItems, _houseViewModel.GarageProductsLocY);
            hardwareItems.Visibility = Visibility.Visible;

        }

        private void ShrinkButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void FundsLabel_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {

        }

        private void FundsLabel_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {
            //TextBox tbox = sender as TextBox;
            ////if (tbox != null && e.LeftButton == MouseButtonState.Pressed)
            ////{
            ////    DragDrop.DoDragDrop(tbox,tbox.Text, DragDropEffects.Move);
            ////}:
            //if (e.LeftButton == MouseButtonState.Pressed)
            //{
            //    // Package the data.
            //    DataObject data = new DataObject();
            //    data.SetData(DataFormats.StringFormat, tbox.Text);

            //    // Initiate the drag-and-drop operation.
            //    DragDrop.DoDragDrop(this, data, DragDropEffects.Copy | DragDropEffects.Move);
            //}

        }
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            cLogger.Log("ImageLivingRoomFileName " + _houseViewModel.ImageLivingRoomFileName);
            string fileName = System.IO.Path.GetFileName(_houseViewModel.ImageLivingRoomFileName);
            Canvas.SetLeft(EntryDoorToGarage, _houseViewModel.GarageButtonLocX);
            Canvas.SetTop(EntryDoorToGarage, _houseViewModel.GarageButtonLocY);
         
        }
        private void FundsLabel_GiveFeedback(object sender, GiveFeedbackEventArgs e)
        {

        }

       
    }
}

