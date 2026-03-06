using CatoriCity2025WPF.Objects;
using CatoriCity2025WPF.Objects.Arguments;
using CommunityToolkit.Mvvm.Messaging;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace CatoriCity2025WPF.Views.Controls
{
    /// <summary>
    /// Interaction logic for HelpIconControl.xaml
    /// </summary>
    public partial class HelpIconControl : UserControl
    {
        public static readonly DependencyProperty CaptionProperty =
            DependencyProperty.Register(nameof(Caption), typeof(string), typeof(HelpIconControl));

        public static readonly DependencyProperty LocationProperty =
            DependencyProperty.Register(nameof(Location), typeof(BaloonLocationEnum), typeof(HelpIconControl));
        
        public static readonly DependencyProperty TitleProperty =
           DependencyProperty.Register(nameof(Title), typeof(string), typeof(HelpIconControl));

        [Description("The Locationof the Help Balloon."), Category("Common Properties")]
        public BaloonLocationEnum Location
        {
            get => (BaloonLocationEnum)GetValue(LocationProperty);
            set => SetValue(LocationProperty, value);
        }
        [Description("The title displayed in the Help Balloon."), Category("Common Properties")]
        public string Title
        {
            get => (string)GetValue(TitleProperty);
            set => SetValue(TitleProperty, value);
        }
        [Description("The caption displayed in the Help Balloon."), Category("Common Properties")]
        public string Caption
        {
            get => (string)GetValue(CaptionProperty);
            set => SetValue(CaptionProperty, value);
        }
      
        public HelpIconControl()
        {
            InitializeComponent();
        }

        private void HelpIconImage_MouseEnter(object sender, MouseEventArgs e)
        {
            double left = Canvas.GetLeft(this);
            double top = Canvas.GetTop(this);
            BaloonMesssage baloonmsg = new BaloonMesssage
            {
                Caption = Caption,
                Title = Title,
                Location = Location,
                Left = left,
                Top = top
            };
            SendMessage(baloonmsg);
        }
        private void SendMessage(BaloonMesssage baloonmsg)
        {

            //Send message to the main window to update the funds
            WeakReferenceMessenger.Default.Send(baloonmsg);
        }

        private void HelpIconImage_MouseLeave(object sender, MouseEventArgs e)
        {

        }

        private void HelpIconImage_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {

        }
    }
}
