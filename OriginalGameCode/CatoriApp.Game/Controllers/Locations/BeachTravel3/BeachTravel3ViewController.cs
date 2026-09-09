using CatoriApp.Game.Objects.Hotspots;
using CatoriApp.Game.Views.Locations.BeachTravel3;
using System.IO;
using System.Windows.Input;

namespace CatoriApp.Game.Controllers.Locations.BeachTravel3;

public sealed class BeachTravel3ViewController : ControllerAnimateLayoutsBase
{
    private readonly BeachTravel3View _view;
    public BeachTravel3ViewController(BeachTravel3View view)
    {
        _view=view??throw new ArgumentNullException(nameof(view));
        var location=new CatoriServices.Objects.database.Locations.LocationRepository().GetByNameAsync("BeachTravel3").GetAwaiter().GetResult()
            ??throw new InvalidOperationException("Location 'BeachTravel3' was not found in the game database.");
        _locationId=location.LocationId;
        LoadBackground();
        _view.LocationUCPanel.Content =
            new CatoriApp.Game.Views.Controls.Locations.Factory.Location_UC((int)_locationId);
        var items=_layoutItemService.GetByLocationIdAsync(_locationId).GetAwaiter().GetResult();
        HotspotLayer.Attach(_view.MainCanvas,items,_view,_productionSession.StartPath);
        _view.PreviewKeyDown+=View_PreviewKeyDown;
    }
    private void LoadBackground()
    {
        string configured="C:\\Development\\Gaming\\Catori2026\\Catori_2026_v1\\Images\\Airports\\BeschTravelAirport.png";
        string path=System.IO.Path.IsPathRooted(configured)?configured:Imagehelper.GetImagePath(System.IO.Path.Combine(GlobalAllApps.ImageFolder,configured));
        if(!string.IsNullOrWhiteSpace(path))_view.BackgroundImage.Source=UIUtility.GetImageControl(path,100,100,0).Source;
    }
    private void View_PreviewKeyDown(object sender,KeyEventArgs e){if(e.Key!=Key.R||!Keyboard.Modifiers.HasFlag(ModifierKeys.Control))return;_productionSession.StartRoots();e.Handled=true;}
    public void Close(){Dispose();_view.Close();}
    public override void Dispose(){_view.PreviewKeyDown-=View_PreviewKeyDown;base.Dispose();}
}
