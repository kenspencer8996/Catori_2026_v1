namespace CatoriApp.Game.Views.Controls.Stores
{
    /// <summary>
    /// Interaction logic for ShelfItemControl.xaml
    /// </summary>
    public partial class ShelfItemControl : UserControl
    {
        public ShelfItemControl()
        {
            InitializeComponent();
            //LotLabel.Content = Street.ToString();
            ShelfLabel.Content = "";
            if (CityScapeGlobal.ShowAllBordersIfAvailable)
            {
                UCBorder.BorderThickness = new System.Windows.Thickness(2);
            }
            else
            {
                UCBorder.BorderThickness = new System.Windows.Thickness(0);
            }
        }
        public bool ShelfOccupied { get; set; } = false;

        public ShopItemControl Shelf { get; set; }
        public ShelfLocationViewModel Model { get;  set; }

        public void AddSHopItemToShelfLocation(UserControl userControl)
        {
            if (userControl != null)
            {
                if (userControl.GetType() == typeof(ShopItemControl))
                {
                    Shelf = userControl as ShopItemControl;
                }
                ShelfUCMainLayout.Children.Add(userControl);
                Canvas.SetLeft(userControl, 0);
                Canvas.SetTop(userControl, 0);
                ShelfOccupied = true;
            }
        }
        public void RemoveBuilding()
        {
            if (ShelfUCMainLayout.Children.Count > 0)
            {
                ShelfUCMainLayout.Children.Clear();
                ShelfOccupied = false;
            }
        }
    }
}


