using CatoriApp.Game.Controllers.Locations.AirportTerminalExterior;

namespace CatoriApp.Game.Views.Locations.AirportTerminalExterior;

public partial class AirportTerminalExteriorView : Window
{
    private readonly AirportTerminalExteriorViewController _controller;
    public AirportTerminalExteriorView()
    {
        InitializeComponent();
        _controller=new AirportTerminalExteriorViewController(this);
    }

    private void ExitButton_Click(object sender,RoutedEventArgs e)
    {
        _controller.Close();
    }

    private void MainCanvas_SizeChanged(object sender,SizeChangedEventArgs e)
    {
        Canvas.SetLeft(ExitButton,Math.Max(0,e.NewSize.Width-ExitButton.Width-8));
    }
}
