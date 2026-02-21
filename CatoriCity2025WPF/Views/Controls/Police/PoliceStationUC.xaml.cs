using CatoriCity2025WPF.Objects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace CatoriCity2025WPF.Views.Controls
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
