using CatoriApp.Game.Controllers.Locations.AirprtTest;
using System.Windows;
using System.Windows.Controls;

namespace CatoriApp.Game.Views.Locations.AirprtTest;

public partial class AirprtTestView : Window
{
    private readonly AirprtTestViewController _controller;
    public AirprtTestView()
    {
        InitializeComponent();
        _controller=new AirprtTestViewController(this);
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
