using System.IO;
using System.Text.Json;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using CatoriApp.Game.ViewModels.Locations;
using CatoriShared.Hotspots;
using CatoriShared.Diagnostics;
using CatoriApp.Core.Objects.DragDrop;
using CatoriUCLibrary.Views.Person;

namespace CatoriApp.Game.Objects.Hotspots;

public static class HotspotLayer
{
    public static IReadOnlyList<Shape> Attach(Canvas canvas,
        IEnumerable<LocationLayoutItemViewModel> items, FrameworkElement owner,
        Func<string,bool>? startPath=null)
    {
        List<Shape> result=[];
        foreach(var item in items.Where(item=>string.Equals(item.MajorItemType,"Hotspot",StringComparison.OrdinalIgnoreCase)))
        {
            HotspotActionDefinition action=Parse(item.MetadataJson);
            HotspotDropShape? shape=CreateShape(item,action,owner,startPath);
            if(shape==null)continue;
            shape.Fill=Brushes.Transparent;
            shape.Stroke=Brushes.Transparent;
            shape.StrokeThickness=2;
            shape.Cursor=Cursors.Hand;
            shape.ToolTip=action.ToolTip;
            shape.Tag=item.ItemName;
            Panel.SetZIndex(shape,Math.Max(3000,item.ZIndex));
            if(action.HighlightOnHover)
            {
                shape.MouseEnter+=(_,_)=>{shape.Fill=new SolidColorBrush(Color.FromArgb(45,0,170,255));shape.Stroke=Brushes.DeepSkyBlue;};
                shape.MouseLeave+=(_,_)=>{shape.Fill=Brushes.Transparent;shape.Stroke=Brushes.Transparent;};
            }
            shape.MouseLeftButtonUp+=(_,e)=>{HotspotActionService.Execute(action,owner,startPath);e.Handled=true;};
            canvas.Children.Add(shape);
            GlobalCode.GetDragmanager(canvas).RegisterDropTarget(shape);
            result.Add(shape);
        }
        return result;
    }

    private static HotspotDropShape? CreateShape(
        LocationLayoutItemViewModel item,
        HotspotActionDefinition action,
        FrameworkElement owner,
        Func<string,bool>? startPath)
    {
        if(item.ItemType==CatoriServices.Objects.Entities.Locations.LocationLayoutItemType.Polygon)
        {
            if(item.Points.Count<3)return null;
            List<Point> points=item.Points.OrderBy(point=>point.PointIndex).Select(point=>new Point(point.X,point.Y)).ToList();
            double left=points.Min(point=>point.X);
            double top=points.Min(point=>point.Y);
            PathFigure figure=new(points[0]-new Vector(left,top),points.Skip(1).Select(point=>new LineSegment(point-new Vector(left,top),true)),true);
            HotspotDropShape polygon=new(new PathGeometry([figure]),action,owner,startPath)
            {
                Width=Math.Max(1,points.Max(point=>point.X)-left),
                Height=Math.Max(1,points.Max(point=>point.Y)-top)
            };
            Canvas.SetLeft(polygon,left);
            Canvas.SetTop(polygon,top);
            return polygon;
        }
        if(item.ItemType!=CatoriServices.Objects.Entities.Locations.LocationLayoutItemType.Rectangle)return null;
        double x=item.X,y=item.Y,width=item.Width,height=item.Height;
        if(item.Points.Count>=2)
        {
            x=item.Points.Min(p=>p.X);y=item.Points.Min(p=>p.Y);
            width=item.Points.Max(p=>p.X)-x;height=item.Points.Max(p=>p.Y)-y;
        }
        width=Math.Max(1,width);
        height=Math.Max(1,height);
        HotspotDropShape rectangle=new(new RectangleGeometry(new Rect(0,0,width,height)),action,owner,startPath)
        {
            Width=width,
            Height=height,
            RenderTransform=new RotateTransform(item.RotationDegrees)
        };
        Canvas.SetLeft(rectangle,x);Canvas.SetTop(rectangle,y);return rectangle;
    }

    private static HotspotActionDefinition Parse(string? json)
    {
        if(string.IsNullOrWhiteSpace(json))return new();
        try{return JsonSerializer.Deserialize<HotspotActionDefinition>(json,new JsonSerializerOptions{PropertyNameCaseInsensitive=true})??new();}
        catch(JsonException ex)
        {
            GameSafetyLog.Error("Hotspots", "Ignored malformed hotspot metadata.", ex);
            return new();
        }
    }
}

internal sealed class HotspotDropShape : Shape, IDropTarget
{
    private readonly Geometry _geometry;
    private readonly HotspotActionDefinition _action;
    private readonly FrameworkElement _owner;
    private readonly Func<string,bool>? _startPath;

    public HotspotDropShape(
        Geometry geometry,
        HotspotActionDefinition action,
        FrameworkElement owner,
        Func<string,bool>? startPath)
    {
        _geometry=geometry;
        _action=action;
        _owner=owner;
        _startPath=startPath;
    }

    protected override Geometry DefiningGeometry => _geometry;

    public bool CanDrop(IDraggable element)
    {
        return element is PersonUC;
    }

    public void OnDrop(IDraggable element)
    {
        HotspotActionService.Execute(_action,_owner,_startPath);
    }

    public void HighlightOn()
    {
        Fill=new SolidColorBrush(Color.FromArgb(45,0,170,255));
        Stroke=Brushes.DeepSkyBlue;
    }

    public void HighlightOff()
    {
        Fill=Brushes.Transparent;
        Stroke=Brushes.Transparent;
    }

    public Point GetSnapPoint(IDraggable dragged)
    {
        return dragged.OriginalPosition;
    }
}

public static class HotspotActionService
{
    public static void Execute(HotspotActionDefinition action,FrameworkElement owner,Func<string,bool>? startPath)
    {
        Window? ownerWindow=Window.GetWindow(owner);
        try
        {
            bool completed = action.ActionType switch
            {
                HotspotActionType.OpenImageWindow => OpenImage(action,ownerWindow),
                HotspotActionType.ShowMessage => ShowConfiguredMessage(action,ownerWindow),
                HotspotActionType.InvokeButton => InvokeButton(action,ownerWindow),
                HotspotActionType.OpenWindow => OpenWindow(action,ownerWindow),
                HotspotActionType.StartPath => !string.IsNullOrWhiteSpace(action.Target)
                    && startPath?.Invoke(action.Target) == true,
                _ => false
            };
            if (!completed)
                ShowUnavailable(ownerWindow, action);
        }
        catch(Exception ex)
        {
            GameSafetyLog.Error("Hotspots",
                $"Hotspot action '{action.ActionType}' targeting '{action.Target}' failed.", ex);
            ShowUnavailable(ownerWindow, action);
        }
    }

    private static bool OpenImage(HotspotActionDefinition action,Window? owner)
    {
        if(string.IsNullOrWhiteSpace(action.ImagePath))return false;
        string path=Environment.ExpandEnvironmentVariables(action.ImagePath);
        if(System.IO.Path.IsPathRooted(path) && !System.IO.File.Exists(path))return false;
        Uri uri=System.IO.Path.IsPathRooted(path)?new Uri(path,UriKind.Absolute):new Uri(path,UriKind.RelativeOrAbsolute);
        Image image=new(){Source=new BitmapImage(uri),Stretch=Stretch.Uniform};
        Window window=new(){Title=action.Title??System.IO.Path.GetFileNameWithoutExtension(path),Width=900,Height=650,Content=image,Owner=owner};
        window.Show();
        return true;
    }

    private static bool OpenWindow(HotspotActionDefinition action,Window? owner)
    {
        if(string.IsNullOrWhiteSpace(action.Target))return false;
        string target=action.Target.Trim();
        string targetViewName=target.EndsWith("View",StringComparison.OrdinalIgnoreCase)?target:target+"View";
        Type? type=AppDomain.CurrentDomain.GetAssemblies()
            .SelectMany(assembly=>GetLoadableTypes(assembly))
            .FirstOrDefault(candidate=>
                string.Equals(candidate.FullName,target,StringComparison.OrdinalIgnoreCase)
                ||string.Equals(candidate.Name,target,StringComparison.OrdinalIgnoreCase)
                ||string.Equals(candidate.Name,targetViewName,StringComparison.OrdinalIgnoreCase));
        if(type!=null&&typeof(Window).IsAssignableFrom(type)&&Activator.CreateInstance(type) is Window window)
        {window.Owner=owner;window.Show();return true;}
        return false;
    }

    private static IEnumerable<Type> GetLoadableTypes(System.Reflection.Assembly assembly)
    {
        try
        {
            return assembly.GetTypes();
        }
        catch(System.Reflection.ReflectionTypeLoadException exception)
        {
            return exception.Types.OfType<Type>();
        }
    }

    private static bool InvokeButton(HotspotActionDefinition action,Window? owner)
    {
        if(FindByName<Button>(owner,action.Target) is not Button button)return false;
        button.RaiseEvent(new RoutedEventArgs(Button.ClickEvent));return true;
    }

    private static bool ShowConfiguredMessage(HotspotActionDefinition action,Window? owner)
    {
        string message=action.Message??action.Target??string.Empty;
        if(string.IsNullOrWhiteSpace(message))return false;
        MessageBox.Show(owner,message,action.Title??"Catori");return true;
    }

    private static void ShowUnavailable(Window? owner,HotspotActionDefinition action)
    {
        GameSafetyLog.Warning("Hotspots",
            $"Unavailable hotspot action '{action.ActionType}' targeting '{action.Target}'.");
        MessageBox.Show(owner,"That area isn’t ready yet.","Catori",
            MessageBoxButton.OK,MessageBoxImage.Information);
    }

    private static T? FindByName<T>(DependencyObject? root,string? name) where T:FrameworkElement
    {
        if(root==null||string.IsNullOrWhiteSpace(name))return null;
        if(root is T match&&string.Equals(match.Name,name,StringComparison.Ordinal))return match;
        for(int i=0;i<VisualTreeHelper.GetChildrenCount(root);i++)if(FindByName<T>(VisualTreeHelper.GetChild(root,i),name) is T found)return found;
        return null;
    }
}
