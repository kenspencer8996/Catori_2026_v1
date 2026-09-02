using CatoriApp.Game.Objects.Hotspots;
using CatoriApp.Game.Views.Locations.factory6;
using System.IO;
using System.Windows.Input;

namespace CatoriApp.Game.Controllers.Locations.factory6;

public sealed class factory6ViewController : ControllerAnimateLayoutsBase
{
    private readonly factory6View _view;
    public factory6ViewController(factory6View view)
    {
        _view=view??throw new ArgumentNullException(nameof(view));
        var location=new CatoriServices.Objects.database.Locations.LocationRepository().GetByNameAsync("factory6").GetAwaiter().GetResult()
            ??throw new InvalidOperationException("Location 'factory6' was not found in the game database.");
        _locationId=location.LocationId;
        LoadBackground();
        _view.LocationUCPanel.Content=new CatoriApp.Game.Views.Controls.Locations.Factory.Location_UC((int)_locationId);
        var items=_layoutItemService.GetByLocationIdAsync(_locationId).GetAwaiter().GetResult();
        HotspotLayer.Attach(_view.MainCanvas,items,_view,_productionSession.StartPath);
        _view.PreviewKeyDown+=View_PreviewKeyDown;
    }
    private void LoadBackground()
    {
        string configured="C:\\Development\\Gaming\\Catori2026\\Catori_2026_v1\\Images\\LocationInteriors\\Factory\\Location6.png";
        string path=System.IO.Path.IsPathRooted(configured)?configured:Imagehelper.GetImagePath(System.IO.Path.Combine(GlobalAllApps.ImageFolder,configured));
        if(!string.IsNullOrWhiteSpace(path))
            _view.BackgroundImage.Source=UIUtility.GetImageControl(path,100,100,0).Source;
    }

    private void View_PreviewKeyDown(object sender,KeyEventArgs e)
    {
        if(e.Key!=Key.R||!Keyboard.Modifiers.HasFlag(ModifierKeys.Control))
            return;
        _productionSession.StartRoots();
        e.Handled=true;
    }

    public void Close()
    {
        Dispose();
        _view.Close();
    }

    public override void Dispose()
    {
        _view.PreviewKeyDown-=View_PreviewKeyDown;
        base.Dispose();
    }
}
