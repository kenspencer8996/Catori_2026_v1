using CatoriCity2025WPF.Objects.Messages;
using CommunityToolkit.Mvvm.Messaging;
using System.Windows.Input;

namespace CatoriCity2025WPF.Views.Controls
{
    /// <summary>
    /// Interaction logic for StoreHardwareControl.xaml
    /// </summary>
    public partial class StoreHardwareControl : UserControl
    {
        ShopItemShowMessage msg = new ShopItemShowMessage();
        bool IsPersonMouseUp = false;
        PersonViewModel _personViewModel;
        bool isMouseOver = false;
        public StoreHardwareControl()
        {
            InitializeComponent();

            WeakReferenceMessenger.Default.Register<PersonMouseMessage>(this, (r, m) =>
            {
                cLogger.Log(this.Name + " WeakReferenceMessenger PersonMouseMessage called : " + "MouseState " + m.MouseState + "  ismouseover " + isMouseOver);
                if (m.MouseState == MouseStatesEnum.Up)
                {
                    if (isMouseOver)
                    {
                    }
                    IsPersonMouseUp = true;
                    _personViewModel = m.Person;
                }
            });
        }
       
        private void DoorImage_MouseDown(object sender, MouseButtonEventArgs e)
        {
            cLogger.Log("event hit");

        }

        private void MainLayout_Drop(object sender, DragEventArgs e)
        {
            cLogger.Log("event hit");

        }

        private void MainBorder_Drop(object sender, DragEventArgs e)
        {
            cLogger.Log("event hit");


        }

        private void DoorImage_Drop(object sender, DragEventArgs e)
        {
            cLogger.Log("event hit");


        }

        private void DoorImage_MouseMove(object sender, MouseEventArgs e)
        {
            cLogger.Log("event hit");

        }

        private void UserControl_MouseMove(object sender, MouseEventArgs e)
        {
            //cLogger.Log("event hit");

        }

        private void UserControl_Drop(object sender, DragEventArgs e)
        {
            //var data = e.Data;
            // if (data != null)
            //{
            //    string dataasstring = data.GetData(DataFormats.StringFormat).ToString();
            //    if (dataasstring == null || dataasstring == "" || dataasstring == "0.0")
            //        return;
            //     PersonViewModel personmodel = GenericSerializer.Deserialize<PersonViewModel>(dataasstring);
            //    msg.Model = personmodel;
            //}
        }

        private void UserControl_MouseEnter(object sender, MouseEventArgs e)
        {
            isMouseOver = true;
            cLogger.Log(this.Name + " event hit : IsPersonMouseUp " + IsPersonMouseUp);
        }

        private void UserControl_MouseLeave(object sender, MouseEventArgs e)
        {
            cLogger.Log("event hit");
            isMouseOver = false;
            IsPersonMouseUp = false;
        }

        private void UserControl_MouseUp(object sender, MouseButtonEventArgs e)
        {
            cLogger.Log("event hit");
        }

        private void UserControl_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            cLogger.Log("event hit PersonDroppedMessage");
        }

        private void UserControl_PreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            cLogger.Log("event hit");
        }

        private void UserControl_PreviewMouseMove(object sender, MouseEventArgs e)
        {
            cLogger.Log("event hit");
        }

        internal void AddPerson(PersonViewModel person)
        {
            ShowHardwareStoreInteriorMessage msg = new ShowHardwareStoreInteriorMessage();
            msg.Model = person;
            WeakReferenceMessenger.Default.Send<ShowHardwareStoreInteriorMessage>(msg);

        }
    }
}
