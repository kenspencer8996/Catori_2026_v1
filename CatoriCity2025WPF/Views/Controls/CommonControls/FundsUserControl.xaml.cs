using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace CatoriApp.Views.Controls
{
    /// <summary>
    /// Interaction logic for FundsUSERCONTROL.xaml
    /// </summary>
    public partial class FundsUserControl : UserControl
    {
        public FundsUserControl()
        {
            InitializeComponent();
        }

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {

        }
        private void OpenButton_Click(object sender, RoutedEventArgs e)
        {
            this.LayoutTransform = new ScaleTransform(3.0, 3.0);

        }
    }
}

