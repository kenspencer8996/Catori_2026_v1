using CatoriCity2025WPF.Controllers;
using CatoriCity2025WPF.Objects.DragDrop;
using CatoriCity2025WPF.Views.Controls.Digging;
using CatoriCity2025WPF.Views.Controls.Treasure;

namespace CatoriCity2025WPF.Views
{
    /// <summary>
    /// Interaction logic for TreasureFieldView.xaml
    /// </summary>
    public partial class TreasureFieldView : Window
    {
        TreasureFieldViewController _controller;

        int landscapegroup ;
        public TreasureFieldView(double width,double height)
        {
            InitializeComponent();

            this.Width = width;
            this.Height = height;
            _controller = new TreasureFieldViewController(this);
            string imagePath = System.IO.Path.Combine(CityScapeGlobal.ImageFolder,"Fields", "FieldEmpty3D.png");
            FieldImage.Source = UIUtility.GetImageControl(imagePath, width, height, 1).Source;

            landscapegroup = _controller.RandomInRangeInt(5, 6);
        }

        private void TreasureField1View_Loaded(object sender, RoutedEventArgs e)
        {
            _controller.LoadLandscapeAsync(landscapegroup);
        }

        private void ExitButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
