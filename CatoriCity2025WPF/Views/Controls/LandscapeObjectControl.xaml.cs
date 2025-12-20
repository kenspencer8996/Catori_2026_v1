using CatoriCity2025WPF.Objects;
using CatoriCity2025WPF.Objects.Arguments;
using CatoriCity2025WPF.Objects.Services;
using CatoriCity2025WPF.ViewModels;
using CatoriServices.Objects;
using CityAppServices.Objects;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;

namespace CatoriCity2025WPF.Views.Controls
{
    /// <summary>
    /// Interaction logic for LandscapeObjectControl.xaml
    /// </summary>
    public partial class LandscapeObjectControl : UserControl
    {
        private DispatcherTimer _longPressTimer;
        private bool _isLongPressTriggered;
        double _FirstXPos, _FirstYPos;
        private bool _isDragging = false;
        public event EventHandler<DragDropChangeArgs> OnDragDropChange;

        public LandscapeObjectControl()
        {
            InitializeComponent();

            if (GlobalStuff.ShowAllBordersIfAvailable)
            {
                MainBorder.BorderThickness = new Thickness(2);
            }
            else
            {
                MainBorder.BorderThickness = new Thickness(0);
            }
            _longPressTimer = new DispatcherTimer
            {
                Interval = TimeSpan.FromMilliseconds(1000) // Set the long press duration (1 second here)
            };
            _longPressTimer.Tick += LongPressTimer_Tick;

        }
        public double xCenter = 0;
        public double yCenter = 0;

        public void SetCenter(double x,double y)
        {
            xCenter = x + this.Width / 2;
            yCenter = y + this.Height / 2;
        }
        private void LandscapeUC_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
        {

        }
        private void LandscapeUC_Loaded(object sender, RoutedEventArgs e)
        {
        }        
        private void LandscapeUC_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            bool mouseIsDown = System.Windows.Input.Mouse.LeftButton == MouseButtonState.Pressed;
            if (mouseIsDown)
            {
                _isLongPressTriggered = false;
                _longPressTimer.Start();
                //cLogger.Log("LandscapeUC_MouseDown ");
                //    cLogger.Log("LandscapeUC_MouseDown mouse is down");
            }

        }


        public string ControlName
        { 
            get
            {
                return this.Name;
            }
            set
            {
                this.Name = value;
                this.ToolTip = value;
                LandscapeImage.ToolTip = value;
            }
        }
        public LocationXYEntity Location { get; internal set; }

        internal void AddImage(string imagename, int imageWidth, int imageHeight)
        {
            //this.Width = imageWidth;
            //this.Height = imageHeight;
            string path = Imagehelper.GetImagePath(imagename);
            // LandscapeImage.Source = UIUtility.GetImageControl(imagename, tentWidth, tentHeight, 0).Source;
            
            LandscapeImage.Source = UIUtility.GetImageControl(path, imageWidth, imageHeight, 0).Source; ;
            
        }

        private void LongPressTimer_Tick(object? sender, EventArgs e)
        {
            cLogger.Log("Long Press Triggered uc size " + Width + "," + Height + "  image size " + LandscapeImage.Width + "," + LandscapeImage.Height); 
            _longPressTimer.Stop();

            bool mouseIsDown = System.Windows.Input.Mouse.LeftButton == MouseButtonState.Pressed;
            if (mouseIsDown)
            {
                _isLongPressTriggered = true;
            }

            if (_isLongPressTriggered)
            {
                _isDragging = true;
                MainBorder.BorderThickness = new Thickness(4);
            }
        }

        private void ResizeMenuItem_Click(object sender, RoutedEventArgs e)
        {
            WidthTextBox.Text = this.Width.ToString();
            HeightTextBox.Text = this.Height.ToString();
            SizePopup.IsOpen = true;
            cLogger.Log("ResizeMenuItem_Click _isDragging:" + _isDragging);

        }

        private void ResizeOKButton_Click(object sender, RoutedEventArgs e)
        {
            SizePopup.IsOpen = false;
            double tempwidth = Convert.ToDouble(WidthTextBox.Text);
            double tempheight = Convert.ToDouble(HeightTextBox.Text);
            if (tempwidth != this.Width)
            {
                this.Width = tempwidth;
            }
            if (tempheight != this.Height)
            {
                this.Height = tempheight;
            }
            var models = from m in GlobalStuff.LandscapeObjects
                         where m.Name == this.Name
                         select m;
            if (models.Count() > 0)
            {

                LandscapeObjectViewModel model = models.First();
                model.Width = tempwidth;
                model.Height = tempheight;
                // _landscapeObjectService.Upsert(model.Entity);
                OnDragDropChange(this, new DragDropChangeArgs());
            }

        }
        private void MoveMenuItem_Click(object sender, RoutedEventArgs e)
        {

        }
        private void LandscapeUC_MouseUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
           CaptureMouse(false);
            _isDragging = false;
            cLogger.Log("LandscapeUC_MouseUp ");
            MainBorder.BorderThickness = new Thickness(0);
            double x = Canvas.GetLeft(this);
            double y = Canvas.GetTop(this);
            cLogger.Log("LandscapeUC_MouseUp " + this.Name + "  x " + x + "  y " + y);
        }
        private void LandscapeUC_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            cLogger.Log("LandscapeUC_MouseLeftButtonDown ");

            _FirstXPos = Canvas.GetLeft(this); 
            _FirstYPos = Canvas.GetTop(this);
            CaptureMouse(true);
            cLogger.Log("LandscapeUC_MouseLeftButtonDown " + this.Name + " x " + _FirstXPos + " y " + _FirstYPos);
        }

        private void LandscapeUC_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {
           // cLogger.Log("LandscapeUC_MouseMove ");
            Canvas canvas = this.Parent as Canvas;
            if (_isDragging)
            {
                //cLogger.Log("  LandscapeUC_MouseMove _isDragging");
                double ctrlwidsize = this.Width; ;
                double halfwidsize = ctrlwidsize / 2;
                double ctrlhghsize = this.Height; ;
                double halfhghsize = ctrlhghsize / 2;
                Point canvasPoint = e.GetPosition(canvas);
                Point objPosition = e.GetPosition((this));
                double left = canvasPoint.X - halfwidsize;
                double top = canvasPoint.Y - halfhghsize;
                if (e.LeftButton == MouseButtonState.Pressed)
                {
                    Canvas.SetLeft(this, left);
                    Canvas.SetTop(this, top);
                    Location.x = Canvas.GetLeft(this);
                    Location.y = Canvas.GetTop(this);
                    OnDragDropChange(this, new DragDropChangeArgs());
                }
            }
        }

        private void LandscapeUC_SizeChanged(object sender, SizeChangedEventArgs e)
        {

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
