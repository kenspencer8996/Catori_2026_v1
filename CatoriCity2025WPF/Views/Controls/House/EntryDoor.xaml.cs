using CatoriApp.Objects.Arguments;
using System.Windows.Input;

namespace CatoriApp.Views.Controls.House
{
    /// <summary>
    /// Interaction logic for EntryDoor.xaml
    /// </summary>
    public partial class EntryDoor : UserControl
    {
        public event EventHandler<DoorOpenEventArgs> OnOpenDoor;

        public EntryDoor()
        {
            InitializeComponent();

            string tooltipimageSaw = System.IO.Path.Combine(GlobalAllApps.ImageFolder, "Stores", "Saw.png");

            ImageTextToolTip toolTip = new ImageTextToolTip
            {
                Title = "Garage",
                Description = "View the garage and select products to use.",
                Icon = UIUtility.GetImageControl(tooltipimageSaw, 32, 32, 0).Source
            };
            this.ToolTip = toolTip;
        }

        private void UserControl_MouseUp(object sender, MouseButtonEventArgs e)
        {
            DoorOpenEventArgs args = new DoorOpenEventArgs();
            OnOpenDoor(this, args);
        }
    }
}

