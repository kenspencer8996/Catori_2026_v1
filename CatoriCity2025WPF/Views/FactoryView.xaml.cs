using CatoriApp.Views.Controls.Factory;
using System.Windows.Media.Imaging;

namespace CatoriApp.Views
{
    /// <summary>
    /// Interaction logic for FactoryView.xaml
    /// </summary>
    public partial class FactoryView : Window
    {
        int _locationNumber;
        public FactoryView(int locationNumber)
        {
            InitializeComponent();
            _locationNumber = locationNumber; 
        }

        private void ExitButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            FactoryInterior_UC locationInterior = new FactoryInterior_UC(_locationNumber);
            locationInterior.Width = this.ActualWidth;
            locationInterior.Height = this.ActualHeight;
            LocationUCPanel.Children.Add(locationInterior);
            //switch (_locationNumber)
            //{
            //    case 1:
            //        LocationInterior_1UC locationInterior_1UC = new LocationInterior_1UC();
            //        locationInterior_1UC.Width = this.ActualWidth;
            //        locationInterior_1UC.Height = this.ActualHeight;
            //        LocationUCPanel.Children.Add(locationInterior_1UC);   
            //        break;
            //    case 2:
            //        LocationInterior_2UC locationInterior_2UC = new LocationInterior_2UC();
            //        locationInterior_2UC.Width = this.ActualWidth;
            //        locationInterior_2UC.Height = this.ActualHeight;
            //        LocationUCPanel.Children.Add(locationInterior_2UC);
            //        break;
            //    case 3:
            //        LocationInterior_3UC locationInterior_3UC = new LocationInterior_3UC();
            //        locationInterior_3UC.Width = this.ActualWidth;
            //        locationInterior_3UC.Height = this.ActualHeight;
            //        LocationUCPanel.Children.Add(locationInterior_3UC);
            //        break;
            //    case 4:
            //        LocationInterior_4UC locationInterior_4UC = new LocationInterior_4UC();
            //        locationInterior_4UC.Width = this.ActualWidth;
            //        locationInterior_4UC.Height = this.ActualHeight;
            //        LocationUCPanel.Children.Add(locationInterior_4UC);
            //        break;
            //    default:
            //        break;
            //}
        }
    }
}

