using System.Windows.Input;

namespace CatoriCity2025WPF.Views.Controls
{
    /// <summary>
    /// Interaction logic for SHopItemControl.xaml
    /// </summary>
    public partial class ShopItemControl : UserControl
    {
        public ShopItemControl()
        {
            InitializeComponent();
          
        }
        private bool _mouaeIsDown = false;  
        // 🔹 Dependency Property
        private bool _isDragging = false;

        private ShopItemViewModel _model;
        public ShopItemViewModel Model 
        { 
            get
            {
                return _model;
            }
            set
            {
                _model = value;
            }
        }

        private void MainBorder_SizeChanged(object sender, SizeChangedEventArgs e)
        {

        }

        private void UC_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            _mouaeIsDown = true;
            CaptureMouse(true);

        }
        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                // Package the data.
                DataObject data = new DataObject();
                string modelasstring = GenericSerializer.Serialize<ShopItemViewModel>(Model);
                data.SetData(DataFormats.StringFormat, modelasstring);

                // Initiate the drag-and-drop operation.
                DragDrop.DoDragDrop(this, data, DragDropEffects.Copy | DragDropEffects.Move);
            }
        }
        private void UC_MouseUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            _mouaeIsDown = false;
        }

        private void UC_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if (_mouaeIsDown)
            {
                _isDragging = true;
                Point currentPosition = e.GetPosition(this.Parent as UIElement);
                double newX = currentPosition.X - (this.ActualWidth / 2);
                double newY = currentPosition.Y - (this.ActualHeight / 2);
                Canvas.SetLeft(this, newX);
                Canvas.SetTop(this, newY);
            }
        }
        private void CaptureMouse(bool capture)
        {
            if (capture)
            {
                this.CaptureMouse();
            }
            else
            {
                this.ReleaseMouseCapture();
            }
        }
    }
}
