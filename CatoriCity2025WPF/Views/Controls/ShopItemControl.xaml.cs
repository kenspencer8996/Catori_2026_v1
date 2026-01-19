using CatoriCity2025WPF.Objects.Arguments;
using System;
using System.Windows.Input;

namespace CatoriCity2025WPF.Views.Controls
{
    /// <summary>
    /// Interaction logic for SHopItemControl.xaml
    /// </summary>
    public partial class ShopItemControl : UserControl
    {
        public event EventHandler<ShopItemControlDrag> StartDrag;
        public event EventHandler<ShopItemControlDrag> StopDrag;
        public event EventHandler<ShopItemControlDrag> ShopItemMouseDown;

        public event EventHandler<ShopItemControlDrag> ShopItemMouseUp;
        public ShopItemControl()
        {
            InitializeComponent();
            cLogger.Log("  started");

        }
        private bool _isMouseDownForDrag = false;
        private Point _mouseOffset;
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

        public void SetLocation(double x, double y)
        {
            double centerX = x - (this.ActualWidth / 2);
            double centerY = y - (this.ActualHeight / 2);
            Canvas.SetLeft(this, centerX);
            Canvas.SetTop(this, centerY);
        }
        private void UC_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            //_isDragging = true;
            //_isMouseDownForDrag = true;
            //_mouseOffset = e.GetPosition(this);
            //HandleCaptureMouse(true);
            Point currentPosition = e.GetPosition(this);
            double xoffset = Math.Abs(currentPosition.X - _mouseOffset.X);
            double yoffset = Math.Abs(currentPosition.Y - _mouseOffset.Y);
            // Check if mouse moved enough to be considered a drag
            cLogger.Log("    xoffset " + xoffset + " yoffset " + yoffset);
            if (xoffset > SystemParameters.MinimumHorizontalDragDistance ||
                yoffset > SystemParameters.MinimumVerticalDragDistance)
            {

                DataObject data = new DataObject();
                string modelasstring = GenericSerializer.Serialize<ShopItemViewModel>(Model);
                UIUtility.SaveToClipboard(modelasstring);
               // data.SetData(DataFormats.StringFormat, modelasstring);

                //// Initiate the drag-and-drop operation.
                //DragDrop.DoDragDrop((DependencyObject)sender, data, DragDropEffects.Copy);

                HandleCaptureMouse(true);
                ShopItemControlDrag args = new ShopItemControlDrag();
                args.shopItemControl = this;
                args.MouseArgs = e;
                ShopItemMouseDown(this, args);
            }

        }
        //protected override void OnMouseMove(MouseEventArgs e)
        //{
        //    base.OnMouseMove(e);
        //    if (e.LeftButton == MouseButtonState.Pressed)
        //    {
        //        cLogger.Log(" in if onmousemove   " );

        //        // Package the data.
        //        DataObject data = new DataObject();
        //        string modelasstring = GenericSerializer.Serialize<ShopItemViewModel>(Model);
        //        data.SetData(DataFormats.StringFormat, modelasstring);

        //        // Initiate the drag-and-drop operation.
        //        DragDrop.DoDragDrop(this, data, DragDropEffects.Copy | DragDropEffects.Move);
        //    }
        //}
        private void UC_MouseUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            cLogger.Log(" in mouseup ");
            //_isDragging = false;
            //StopDrag(this, new ShopItemontrolDrag() { shopItemControl = null });
            HandleCaptureMouse(false);
            ShopItemControlDrag args = new ShopItemControlDrag();
            args.shopItemControl = null;
            ShopItemMouseUp(this, args);
        }

        private void UC_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {
           
        }

       

        private void HandleCaptureMouse(bool capture)
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
