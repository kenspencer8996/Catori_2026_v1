using CatoriCity2025WPF.Objects.DragDrop;
using CatoriCity2025WPF.Objects.Messages;
using CommunityToolkit.Mvvm.Messaging;
using System.Windows.Input;
using System.Windows.Media.Effects;

namespace CatoriCity2025WPF.Views.Controls
{
    /// <summary>
    /// Interaction logic for StoreHardwareControl.xaml
    /// </summary>
    public partial class PostOfficeControl : UserControl,IDropTarget
    {
        bool IsPersonMouseUp = false;
        PersonViewModel _personViewModel;

        public PostOfficeControl()
        {
            InitializeComponent();

            WeakReferenceMessenger.Default.Register<PersonMouseMessage>(this, (r, m) =>
            {
                cLogger.Log(this.Name + " WeakReferenceMessenger called : " + "MouseState " + m.MouseState);
                if (m.MouseState == MouseStatesEnum.Up)
                {
                    IsPersonMouseUp = true;
                    _personViewModel = m.Person;
                }
            });
        }
       
        private void DoorImage_MouseDown(object sender, MouseButtonEventArgs e)
        {
            cLogger.Log("Event Hit");

        }

        private void MainLayout_Drop(object sender, DragEventArgs e)
        {
            cLogger.Log("Event Hit");

        }

        private void MainBorder_Drop(object sender, DragEventArgs e)
        {
            cLogger.Log("Event Hit");

        }

        private void DoorImage_Drop(object sender, DragEventArgs e)
        {
            cLogger.Log("Event Hit");


        }


        private void UserControl_MouseMove(object sender, MouseEventArgs e)
        {

        }

        private void UserControl_MouseEnter(object sender, MouseEventArgs e)
        {
            cLogger.Log(this.Name + " UserControl_MouseEnter called : IsPersonMouseUp " + IsPersonMouseUp);
            if (IsPersonMouseUp)
            {
                PostOfficeInteriorhowMessage msg = new PostOfficeInteriorhowMessage(_personViewModel);

                WeakReferenceMessenger.Default.Send<PostOfficeInteriorhowMessage>(msg);
            }   
        }

        private void UserControl_MouseLeave(object sender, MouseEventArgs e)
        {
            cLogger.Log("Event Hit");

            IsPersonMouseUp = false;
        }

        private void UserControl_PreviewMouseMove(object sender, MouseEventArgs e)
        {
            cLogger.Log("Event Hit");

        }

        public bool CanDrop(IDraggable element)
        {
            return true;
        }

        public void OnDrop(IDraggable element)
        {
            
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

       
        public Point GetSnapPoint(IDraggable dragged)
        {
            var feDragged = (FrameworkElement)dragged;
            double x = Canvas.GetLeft(this) + (this.ActualWidth - feDragged.ActualWidth) / 2;
            double y = Canvas.GetTop(this) + (this.ActualHeight - feDragged.ActualHeight) / 2;
            return new Point(x, y);
        }
    }
}
