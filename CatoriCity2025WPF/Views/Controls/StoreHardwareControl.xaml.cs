using CommunityToolkit.Mvvm.Messaging;
using System.Windows.Input;

namespace CatoriCity2025WPF.Views.Controls
{
    /// <summary>
    /// Interaction logic for StoreHardwareControl.xaml
    /// </summary>
    public partial class StoreHardwareControl : UserControl
    {
        public StoreHardwareControl()
        {
            InitializeComponent();
        }

        private void DoorImage_MouseDown(object sender, MouseButtonEventArgs e)
        {

        }

        private void MainLayout_Drop(object sender, DragEventArgs e)
        {

        }

        private void MainBorder_Drop(object sender, DragEventArgs e)
        {

        }

        private void DoorImage_Drop(object sender, DragEventArgs e)
        {
    

        }

        private void DoorImage_MouseMove(object sender, MouseEventArgs e)
        {

        }

        private void UserControl_MouseMove(object sender, MouseEventArgs e)
        {

        }

        private void UserControl_Drop(object sender, DragEventArgs e)
        {
            var data = e.Data;
            ShopItemShowMessage msg = new ShopItemShowMessage();
            if (data != null)
            {
                string dataasstring = data.GetData(DataFormats.StringFormat).ToString();
                if (dataasstring == null || dataasstring == "" || dataasstring == "0.0")
                    return;
                 PersonViewModel personmodel = GenericSerializer.Deserialize<PersonViewModel>(dataasstring);
                msg.Model = personmodel;
            }

            WeakReferenceMessenger.Default.Send(msg);

        }
    }
}
