using CatoriApp.Game.Controllers.Locations.ConstructionField;
using System.Windows;

namespace CatoriApp.Game.Views.Locations.ConstructionField
{
    public partial class ConstructionFieldView : Window
    {
        private readonly ConstructionFieldViewController _controller;

        public ConstructionFieldView()
        {
            InitializeComponent();
            _controller = new ConstructionFieldViewController(this);
        }

        private void ExitButton_Click(object sender, RoutedEventArgs e)
        {
            _controller.Close();
        }

        private void MainCanvas_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            Canvas.SetLeft(ExitButton, Math.Max(0, e.NewSize.Width - ExitButton.Width - 8));
        }
    }
}
