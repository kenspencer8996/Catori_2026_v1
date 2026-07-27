using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace CatoriApp.Game.Views.Controls
{
    /// <summary>
    /// Interaction logic for ProductUC.xaml
    /// </summary>
    public partial class ProductUC : UserControl
    {
        public ProductUC(string filesubPath)
        {
            InitializeComponent();

            string imagePath = Imagehelper.GetImagePath(filesubPath);
            ProductImage.Source = UIUtility.GetImageControl(imagePath, 15, 15, 0).Source;

        }
    }
}
