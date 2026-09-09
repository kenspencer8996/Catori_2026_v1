using CatoriApp.Game.Views.Locations.ConstructionField;
using System.Windows.Input;

namespace CatoriApp.Game.Controllers.Locations.ConstructionField
{
    public sealed class ConstructionFieldViewController : ControllerAnimateLayoutsBase
    {
        private readonly ConstructionFieldView _view;

        public ConstructionFieldViewController(ConstructionFieldView view)
        {
            _view = view ?? throw new ArgumentNullException(nameof(view));
            LoadBackgroundImage();
            _view.PreviewKeyDown += View_PreviewKeyDown;
        }

        private void LoadBackgroundImage()
        {
            string imagePath = Imagehelper.GetImagePath(System.IO.Path.Combine(
                GlobalAllApps.ImageFolder, "ConstructionFields", "ConstructionLot.png"));
            _view.ConstructionFieldImage.Source = UIUtility.GetImageControl(imagePath, 100, 100, 0).Source;
        }

        private void View_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key != Key.R || !Keyboard.Modifiers.HasFlag(ModifierKeys.Control))
                return;

            _productionSession.StartRoots();
            e.Handled = true;
        }

        public void Close()
        {
            Dispose();
            _view.Close();
        }

        public override void Dispose()
        {
            _view.PreviewKeyDown -= View_PreviewKeyDown;
            base.Dispose();
        }
    }
}
