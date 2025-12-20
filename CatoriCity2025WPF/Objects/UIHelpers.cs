using System.Windows.Controls;

namespace CatoriCity2025WPF.Objects
{
    internal class UIHelpers
    {
        internal static void LoadConent()
        {
            double masterHeight = 600;
            double masterWidth = 1200;
            //GlobalStuff.CityContent = new CityAppContent();
            //GlobalStuff.CityContent.HeightRequest = masterHeight;
            //GlobalStuff.CityContent.WidthRequest = masterWidth;
            //GlobalStuff.CityContent.ZIndex = 1;
            //GlobalStuff.CityLayout = new AbsoluteLayout();
            //GlobalStuff.CityLayout.HorizontalOptions = LayoutOptions.Start;
            //GlobalStuff.CityLayout.VerticalOptions = LayoutOptions.Start;
            //GlobalStuff.CityLayout.HeightRequest = masterHeight;
            //GlobalStuff.CityLayout.WidthRequest = 1200;
            //GlobalStuff.CityLayout.ZIndex = 2;

            //GlobalStuff.CityContent.Content = GlobalStuff.CityLayout;
            // contentView.Content = testlabel;
            Image image = new Image();
            image.Height = masterHeight;
            image.Width = masterWidth;
            image.Source = Imagehelper.GetImageSource("greenyard.jpg");
            image.z = 3;
            GlobalStuff.CityLayout.Children.Add(image);
            AbsoluteLayout.SetLayoutBounds(image, GetRect(0, 0, masterWidth, masterHeight));
            Border border = new Border();
            border.HeightRequest = masterHeight;
            border.WidthRequest = masterWidth;
            GlobalStuff.CityLayout.Children.Add(border);
            // AbsoluteLayout.SetLayoutBounds(border, GetRect(0, 0, 0, 0));
            border.ZIndex = 4;
            GlobalStuff.CityCanvas = new SKCanvasView();
            GlobalStuff.CityCanvas.HeightRequest = masterHeight;
            GlobalStuff.CityCanvas.WidthRequest = masterWidth;
            AbsoluteLayout.SetLayoutBounds(GlobalStuff.CityCanvas, GetRect(0, 0, masterWidth, masterHeight));
            //GlobalStuff._menuControl = new Views.MenuLayoutContent();
            //GlobalStuff._menuControl.IsVisible = false;
            //AbsoluteLayout.SetLayoutBounds(GlobalStuff._menuControl, GetRect(0, 0, 300, 200));
            //cityStreetMaster.CityLayout.Children.Add(GlobalStuff._menuControl);
            //return cityStreetMaster;
        }
        internal static Rect GetRect(double x, double y, double width, double height)
        {
            Rect rect = new Rect();
            rect.X = x;
            rect.Y = y;
            rect.Width = width;
            rect.Height = height;
            return rect;
        }
    }
}
