using CatoriServices.Objects.database.People;
using CatoriUCLibrary.Views.Person;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace CatoriApp.Views.Shared;

public sealed class AvatarSelectorWindow:Window
{
    private static AvatarSelectorWindow? _open;
    private readonly AvatarRepository _repository=new();
    private readonly ListBox _list=new(){DisplayMemberPath=nameof(AvatarEntity.Name),Margin=new Thickness(8)};

    private AvatarSelectorWindow(Window owner)
    {
        Owner=owner;Title="Choose Avatar";Width=330;Height=500;ResizeMode=ResizeMode.CanResizeWithGrip;
        WindowStartupLocation=WindowStartupLocation.Manual;Left=Math.Max(0,owner.Left+owner.ActualWidth-Width-24);Top=owner.Top+24;
        var root=new DockPanel();var heading=new TextBlock{Text="Current avatar",FontSize=18,FontWeight=FontWeights.Bold,Margin=new Thickness(12)};DockPanel.SetDock(heading,Dock.Top);root.Children.Add(heading);
        var buttons=new StackPanel{Orientation=Orientation.Horizontal,HorizontalAlignment=HorizontalAlignment.Right,Margin=new Thickness(8)};DockPanel.SetDock(buttons,Dock.Bottom);
        var apply=new Button{Content="Apply",Width=90,Margin=new Thickness(0,0,8,0),IsDefault=true};var close=new Button{Content="Close",Width=90,IsCancel=true};apply.Click+=ApplyClick;close.Click+=(_,_)=>Close();buttons.Children.Add(apply);buttons.Children.Add(close);root.Children.Add(buttons);
        _list.MouseDoubleClick+=ApplyClick;root.Children.Add(_list);Content=root;Closed+=(_,_)=>_open=null;
        Reload();
    }
    public static void ShowFor(Window owner)
    {
        if(_open!=null){_open.Owner=owner;_open.Activate();return;}
        _open=new(owner);_open.Show();
    }
    public static void ApplyCurrentTo(DependencyObject root)
    {
        try
        {
            var repository=new AvatarRepository();string? name=repository.GetCurrentName();if(string.IsNullOrWhiteSpace(name))return;
            AvatarEntity? avatar=repository.GetByName(name);if(avatar!=null)ApplyAvatar(root,avatar.SettingsJson);
        }
        catch(Exception ex){System.Diagnostics.Debug.WriteLine($"Could not load current avatar: {ex.Message}");}
    }
    private void Reload(){var values=_repository.GetAll();_list.ItemsSource=values;string? current=_repository.GetCurrentName();_list.SelectedItem=values.FirstOrDefault(x=>string.Equals(x.Name,current,StringComparison.OrdinalIgnoreCase));}
    private void ApplyClick(object? sender,RoutedEventArgs e)
    {
        if(_list.SelectedItem is not AvatarEntity avatar)return;
        _repository.SetCurrentName(avatar.Name);
        foreach(Window window in Application.Current.Windows)if(window is not AvatarSelectorWindow)ApplyAvatar(window,avatar.SettingsJson);
    }
    private static void ApplyAvatar(DependencyObject root,string json)
    {
        if(root is PersonUC person)person.AvatarSettingsJson=json;
        int count=VisualTreeHelper.GetChildrenCount(root);
        for(int i=0;i<count;i++)ApplyAvatar(VisualTreeHelper.GetChild(root,i),json);
    }
}
