using CatoriApp.Game.Controllers.Locations.AirportTicketCounterArea;
using System.Windows;
using System.Windows.Controls;

namespace CatoriApp.Game.Views.Locations.AirportTicketCounterArea;

public partial class AirportTicketCounterAreaView : Window
{
    private readonly AirportTicketCounterAreaViewController _controller;
    public AirportTicketCounterAreaView()
    {
        InitializeComponent();
        _controller=new AirportTicketCounterAreaViewController(this);
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
