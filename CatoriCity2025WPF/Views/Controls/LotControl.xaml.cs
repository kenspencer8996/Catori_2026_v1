using CatoriApp.Objects.DragDrop;
using System.Windows.Media.Effects;

namespace CatoriApp.Views.Controls
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

      
        public void HighlightOn()
        {
            // Example glow
            this.Effect = new DropShadowEffect
            {
                Color = Colors.Gold,
                BlurRadius = 25,
                ShadowDepth = 0,
                Opacity = 0.8
            };
        }

        public void HighlightOff()
        {
            this.Effect = null;
        }

       

        
        public bool CanDrop(IDraggable element)
        {
            return true;
        }

        public void OnDrop(IDraggable element)
        {
            // Building.AddDroppedElement(element) ;
            BuildingUC.AddDroppedElement(element);
        }
        public Point GetSnapPoint(UIElement dragged)
        {
            var feDragged = (FrameworkElement)dragged;
            double x = Canvas.GetLeft(this) + (this.ActualWidth - feDragged.ActualWidth) / 2;
            double y = Canvas.GetTop(this) + (this.ActualHeight - feDragged.ActualHeight) / 2;
            return new Point(x, y);
        }
        public Point GetSnapPoint(IDraggable dragged)
        {
            var feDragged = (FrameworkElement)dragged;
            double x = Canvas.GetLeft(this) + (this.ActualWidth - feDragged.ActualWidth) / 2;
            double y = Canvas.GetTop(this) + (this.ActualHeight - feDragged.ActualHeight) / 2;
            return new Point(x, y);

        }
    }
}

