using CatoriCity2025WPF.Objects.DragDrop;

namespace CatoriCity2025WPF.Views.Controls
{
    /// <summary>
    /// Interaction logic for LotContent.xaml
    /// </summary>
    public partial class LotControl : UserControl,IDropTarget
    {
        public bool IsPrimaryPersonHouse { get; set; } = false;  
        public LotControl()
        {
            InitializeComponent();

            //LotLabel.Content = Street.ToString();
            LotLabel.Content = "";
            if (CityScapeGlobal.ShowAllBordersIfAvailable)
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

        public HouseControl Building { get; set; }
        public IDropAddToUC BuildingUC { get; set; }
        public void AddBuilding(UserControl userControl,bool primary)
        {
            BuildingUC = userControl as IDropAddToUC;
            if (userControl != null)
            {
                if(userControl.GetType() == typeof(HouseControl))
                {
                    Building = userControl as HouseControl;
                }
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

        public bool CanDrop(UIElement element)
        {
            return true;
        }

        public void OnDrop(UIElement element)
        {
           // Building.AddDroppedElement(element) ;
           BuildingUC.AddDroppedElement(element);
        }
    }
}
