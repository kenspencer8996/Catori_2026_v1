using CatoriCity2025WPF.Objects.Arguments;
using System.Windows.Input;

namespace CatoriCity2025WPF.Views.Controls
{
    /// <summary>
    /// Interaction logic for HouseControl.xaml
    /// </summary>
    public partial class RealEstateControl : UserControl
    {
        private string _OfficeImageName = "";
        private Border _border = new Border();
        public event EventHandler<BuildingOpenEventArgs> OnBuildingOpenContent;
        private string PersonName = "";
        public RealEstateControl()
        {
            InitializeComponent();
            string tooltipimagepath = System.IO.Path.Combine(GlobalAllApps.ImageFolder,"Houses", "ForSale.png");

            ImageTextToolTip toolTip = new ImageTextToolTip
            {
                Title = "Realestate office",
                Description = "Open to purchase house.",
                Icon = UIUtility.GetImageControl(tooltipimagepath, 32, 32, 0).Source
            };
            this.ToolTip = toolTip;
        }


        private LotEntity ContentLocationLot;

      
        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            if (CityScapeGlobal.ShowAllBordersIfAvailable)
            {
                MainBorder.BorderThickness = new Thickness(2);
            }
            else
            {
                MainBorder.BorderThickness = new Thickness(0);
                //border.Background = ;
            }
            string filepath = Imagehelper.GetImagePath("houses\\RealEstateOfficeExterior.png");
            OfficeImage.Source = UIUtility.GetImageControl(filepath, 100, 100, 0).Source;

        }




        private void HouseControl_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            double newWidth = e.NewSize.Width;
            double newHeight = e.NewSize.Height;
            double newimagesize = newWidth - 25;
            OfficeImage.Width = newimagesize;
            OfficeImage.Height = newimagesize;
 
        }

        private void fundsThumb_DragDelta(object sender, System.Windows.Controls.Primitives.DragDeltaEventArgs e)
        {

        }

        private void fundsThumb_DragStarted(object sender, System.Windows.Controls.Primitives.DragStartedEventArgs e)
        {
            //var data = new DataObject(DataFormats.StringFormat, Funds);
            //DragDrop.DoDragDrop(fundsThumb, data, DragDropEffects.Move); // Here the exception occurs
        }

      
        
        private void MainLayout_Drop(object sender, DragEventArgs e)
        {
            var data = e.Data;
            if (data != null)
            {
                string dataasstring = data.GetData(DataFormats.StringFormat).ToString();
                if (dataasstring == null || dataasstring == "" || dataasstring == "0.0")
                    return;
                

            }
        }

        private void PersonImage_MouseDown(object sender, MouseButtonEventArgs e)
        {
            cLogger.Log($"HouseControl PersonImage_MouseDown before if :  {PersonName}");
          
            e.Handled = false;
        }

        private void fundsThumb_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            //FundsDetailView fundsDetailView = new FundsDetailView(personmodel);
            //fundsDetailView.Owner = GlobalStuff.MainView;
            //fundsDetailView.ShowDialog();
        }

        private void OfficeImage_MouseDown(object sender, MouseButtonEventArgs e)
        {
            RealestateInteriorView view = new RealestateInteriorView();
            view.Owner = CityScapeGlobal.CityScapeView;
            view.Show();

        }

        private void HouseUC_Loaded(object sender, RoutedEventArgs e)
        {

        }
    }
}
