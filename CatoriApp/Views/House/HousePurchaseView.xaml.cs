using CatoriApp.Controllers;
namespace CatoriApp.Views.House
{
    /// <summary>
    /// Interaction logic for HousePurchaseView.xaml
    /// </summary>
    public partial class HousePurchaseView : Window
    {
        HousePurchaseViewController _controller;
        public HousePurchaseView(HouseViewModel model)
        {
            InitializeComponent();

            _controller = new HousePurchaseViewController(this, model);
        }
    }
}


