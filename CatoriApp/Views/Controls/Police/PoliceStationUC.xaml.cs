namespace CatoriApp.Views.Controls.Police
{
    /// <summary>
    /// Interaction logic for PoliceStationUC.xaml
    /// </summary>
    public partial class PoliceStationUC : UserControl
    {
        public PoliceStationUC()
        {
            InitializeComponent();
            string imagepath = Imagehelper.GetImagePath("policestation_.png");
            PoliceStationImage.Source = UIUtility.GetImageControl(imagepath, 50, 50, 0).Source;
        }
        public LotControl LotControl { get; set; }

        private void MainLayout_Drop(object sender, DragEventArgs e)
        {

        }

        private void MainBorder_Drop(object sender, DragEventArgs e)
        {

        }
    }
}


