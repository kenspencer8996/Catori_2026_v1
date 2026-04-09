using System.Windows.Media.Imaging;

namespace CatoriCity2025WPF.Views
{
    /// <summary>
    /// Interaction logic for FactoryView.xaml
    /// </summary>
    public partial class FactoryView : Window
    {
        int _factoryNumber;
        public FactoryView(int factoryNumber)
        {
            InitializeComponent();
            _factoryNumber = factoryNumber; 
        }

        private void ExitButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            switch (_factoryNumber)
            {
                case 1:
                    FactoryInterior_1UC factoryInterior_1UC = new FactoryInterior_1UC();
                    factoryInterior_1UC.Width = this.ActualWidth;
                    factoryInterior_1UC.Height = this.ActualHeight;
                    FactoryUCPanel.Children.Add(factoryInterior_1UC);   
                    break;
                case 2:
                    FactoryInterior_2UC factoryInterior_2UC = new FactoryInterior_2UC();
                    factoryInterior_2UC.Width = this.ActualWidth;
                    factoryInterior_2UC.Height = this.ActualHeight;
                    FactoryUCPanel.Children.Add(factoryInterior_2UC);
                    break;
                case 3:
                    FactoryInterior_3UC factoryInterior_3UC = new FactoryInterior_3UC();
                    factoryInterior_3UC.Width = this.ActualWidth;
                    factoryInterior_3UC.Height = this.ActualHeight;
                    FactoryUCPanel.Children.Add(factoryInterior_3UC);
                    break;
                case 4:
                    FactoryInterior_4UC factoryInterior_4UC = new FactoryInterior_4UC();
                    factoryInterior_4UC.Width = this.ActualWidth;
                    factoryInterior_4UC.Height = this.ActualHeight;
                    FactoryUCPanel.Children.Add(factoryInterior_4UC);
                    break;
                default:
                    break;
            }
        }
    }
}
