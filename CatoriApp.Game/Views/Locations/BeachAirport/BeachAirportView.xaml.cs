using CatoriApp.Game.Controllers.Locations.BeachAirport;
using System.Windows;
using System.Windows.Controls;

namespace CatoriApp.Game.Views.Locations.BeachAirport;

public partial class BeachAirportView : Window
{
    private readonly BeachAirportViewController _controller;
    public BeachAirportView(){InitializeComponent();_controller=new BeachAirportViewController(this);}
    private void ExitButton_Click(object sender,RoutedEventArgs e)=>_controller.Close();
    private void MainCanvas_SizeChanged(object sender,SizeChangedEventArgs e)=>Canvas.SetLeft(ExitButton,Math.Max(0,e.NewSize.Width-ExitButton.Width-8));
}
