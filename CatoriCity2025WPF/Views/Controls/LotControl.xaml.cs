using CatoriCity2025WPF.Objects;
using System.Windows.Controls;
using static System.Reflection.Metadata.BlobBuilder;

namespace CatoriCity2025WPF.Views.Controls
{
    /// <summary>
    /// Interaction logic for LotContent.xaml
    /// </summary>
    public partial class LotControl : UserControl
    {
        public LotControl()
        {
            InitializeComponent();

            //LotLabel.Content = Street.ToString();
            LotLabel.Content = "";
            if (GlobalStuff.ShowAllBordersIfAvailable)
            {
                LotUCBorder.BorderThickness =new System.Windows.Thickness( 2);
            }
            else
            {
                LotUCBorder.BorderThickness = new System.Windows.Thickness(0);
            }
        }
        public StreetsEnum  Street { get;set; }
        public bool LotOccupied { get; set; } = false;  

        public void AddBuilding(UserControl userControl)
        {
            if (userControl != null)
            {
                LotUCMainLayout.Children.Add(userControl);
                Canvas.SetLeft(userControl, 0);
                Canvas.SetTop(userControl, 0);
                LotOccupied = true;
            }
        }
        public void RemoveBuilding()
        {
            if (LotUCMainLayout.Children.Count >0)
            {
                LotUCMainLayout.Children.Clear();
                LotOccupied = false;
            }
        }
    }
}
