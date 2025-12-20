using CatoriCity2025WPF.Objects;
using CatoriCity2025WPF.ViewModels;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Shapes;

namespace CatoriCity2025WPF.Views
{
    /// <summary>
    /// Interaction logic for DetailHouseInsideView.xaml
    /// </summary>
    public partial class DetailHouseInsideView : Window
    {
        HouseViewModel _houseViewModel;
        public DetailHouseInsideView(HouseViewModel houseViewModel)
        {
            InitializeComponent();
            _houseViewModel = houseViewModel;
            double thisWith = this.Width;
            double thisHeight = this.Height;

            HouseLivingRoomImage.Source =UIUtility.GetImageControl( _houseViewModel.ImageLivingRoomFileName,thisHeight,thisWith,1).Source;
            HouseLivingRoomImage.Width = Width;
            HouseLivingRoomImage.Height = Height;
            double buttonloc = this.Width;
            buttonloc = buttonloc - 30;
            Canvas.SetLeft(ShrinkButton, buttonloc);
            DataContext = _houseViewModel;
        }

        private void ShrinkButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void FundsLabel_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {

        }

        private void FundsLabel_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {
            //TextBox tbox = sender as TextBox;
            ////if (tbox != null && e.LeftButton == MouseButtonState.Pressed)
            ////{
            ////    DragDrop.DoDragDrop(tbox,tbox.Text, DragDropEffects.Move);
            ////}
            //if (e.LeftButton == MouseButtonState.Pressed)
            //{
            //    // Package the data.
            //    DataObject data = new DataObject();
            //    data.SetData(DataFormats.StringFormat, tbox.Text);

            //    // Initiate the drag-and-drop operation.
            //    DragDrop.DoDragDrop(this, data, DragDropEffects.Copy | DragDropEffects.Move);
            //}
        }

        private void FundsLabel_GiveFeedback(object sender, GiveFeedbackEventArgs e)
        {

        }
    }
}
