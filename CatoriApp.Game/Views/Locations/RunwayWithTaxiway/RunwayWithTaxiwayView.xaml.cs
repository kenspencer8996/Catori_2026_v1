using CatoriApp.Game.Controllers.Locations.RunwayWithTaxiway;
using System.Windows;
using System.Windows.Controls;

namespace CatoriApp.Game.Views.Locations.RunwayWithTaxiway;

public partial class RunwayWithTaxiwayView : Window
{
    private readonly RunwayWithTaxiwayViewController _controller;
    public RunwayWithTaxiwayView(){InitializeComponent();_controller=new RunwayWithTaxiwayViewController(this);}
    private void ExitButton_Click(object sender,RoutedEventArgs e)=>_controller.Close();
    private void MainCanvas_SizeChanged(object sender,SizeChangedEventArgs e)=>Canvas.SetLeft(ExitButton,Math.Max(0,e.NewSize.Width-ExitButton.Width-8));
}
