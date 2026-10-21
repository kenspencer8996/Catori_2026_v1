using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Text.Json;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using CatoriInterfaces.AvatarOutfits;
using CatoriUCLibrary.Objects.Outfits;
namespace CatoriUCLibrary.Views;
public partial class ClothesTreeUC : UserControl
{
    private AvatarWardrobeStore? store;
    private bool canSave;
    private Point dragStart;
    private AvatarGarment? draggedGarment;
    private OutfitEditorViewModel? draggedOutfit;
    public ObservableCollection<OutfitEditorViewModel> Outfits { get; } = [];
    public string? WardrobeFolder
    {
        get => (string?)GetValue(WardrobeFolderProperty);
        set => SetValue(WardrobeFolderProperty, value);
    }
    public static readonly DependencyProperty WardrobeFolderProperty = DependencyProperty.Register(nameof(WardrobeFolder), typeof(string), typeof(ClothesTreeUC), new PropertyMetadata(null));
    public ClothesTreeUC()
    {
        InitializeComponent();
        OutfitList.ItemsSource = Outfits;
        Loaded += (_, _) => Reload();
    }
    public void Reload()
    {
        RunAction(() =>
        {
            canSave = false;
            store = new AvatarWardrobeStore(WardrobeFolder);
            var outfits = store.Load();
            foreach (var item in Outfits)
            {
                item.Edited -= Outfit_Edited;
            }
            Outfits.Clear();
            foreach (var outfit in outfits)
            {
                AddEditor(outfit);
            }
            canSave = true;
            var catalog = new AvatarGarmentCatalog();
            GarmentList.ItemsSource = catalog.Load(store.ComponentsFolder);
            StatusText.Text = catalog.Errors.Count == 0
                ? "Add downloaded garment folders to Components, then press Refresh. Outfits save automatically."
                : string.Join(Environment.NewLine, catalog.Errors);
        });
    }
    public OutfitEditorViewModel CreateOutfit(string name)
    {
        if (!canSave)
        {
            throw new InvalidOperationException("The wardrobe could not be loaded. Fix outfits.json and press Refresh before saving.");
        }
        var editor = AddEditor(new AvatarOutfit { Name = name });
        OutfitList.SelectedItem = editor;
        SaveOutfits();
        return editor;
    }
    public void AddGarment(OutfitEditorViewModel outfit, AvatarGarment garment)
    {
        if (!Outfits.Contains(outfit))
        {
            throw new ArgumentException("Select a hanger on this rack.", nameof(outfit));
        }
        outfit.AddGarment(garment);
    }
    private OutfitEditorViewModel AddEditor(AvatarOutfit outfit)
    {
        var editor = new OutfitEditorViewModel(outfit);
        editor.Edited += Outfit_Edited;
        Outfits.Add(editor);
        return editor;
    }
    private void SaveOutfits()
    {
        if (!canSave || store == null)
        {
            throw new InvalidOperationException("The wardrobe is not ready to save.");
        }
        store.Save(Outfits.Select(item => item.Outfit));
        StatusText.Text = "Outfits saved. Drag a hanger's picture onto an avatar to wear it.";
    }
    private void Outfit_Edited(object? sender, EventArgs e)
    {
        RunAction(SaveOutfits);
    }
    private void NewOutfit_Click(object sender, RoutedEventArgs e)
    {
        RunAction(() => CreateOutfit("New outfit"));
    }
    private void SaveOutfits_Click(object sender, RoutedEventArgs e)
    {
        RunAction(SaveOutfits);
    }
    private void DeleteOutfit_Click(object sender, RoutedEventArgs e)
    {
        RunAction(() =>
        {
            if (OutfitList.SelectedItem is OutfitEditorViewModel outfit)
            {
                outfit.Edited -= Outfit_Edited;
                Outfits.Remove(outfit);
                SaveOutfits();
            }
        });
    }
    private void Refresh_Click(object sender, RoutedEventArgs e)
    {
        Reload();
    }
    private void OpenComponentsFolder_Click(object sender, RoutedEventArgs e)
    {
        RunAction(() =>
        {
            string folder = (store ?? new AvatarWardrobeStore(WardrobeFolder)).ComponentsFolder;
            Directory.CreateDirectory(folder);
            Process.Start(new ProcessStartInfo(folder) { UseShellExecute = true });
        });
    }
    private void Garment_MouseDown(object sender, MouseButtonEventArgs e)
    {
        draggedGarment = (sender as FrameworkElement)?.DataContext as AvatarGarment;
        dragStart = e.GetPosition(this);
    }
    private void Garment_MouseMove(object sender, MouseEventArgs e)
    {
        if (draggedGarment == null || !ShouldDrag(e))
        {
            return;
        }
        var garment = draggedGarment;
        draggedGarment = null;
        DragDrop.DoDragDrop((DependencyObject)sender, new DataObject(AvatarOutfitDragFormats.Garment, AvatarWardrobeStore.SnapshotGarment(garment)), DragDropEffects.Copy);
    }
    private void Outfit_MouseDown(object sender, MouseButtonEventArgs e)
    {
        draggedOutfit = (sender as FrameworkElement)?.DataContext as OutfitEditorViewModel;
        dragStart = e.GetPosition(this);
    }
    private void Outfit_MouseMove(object sender, MouseEventArgs e)
    {
        if (draggedOutfit == null || !ShouldDrag(e))
        {
            return;
        }
        var outfit = draggedOutfit;
        draggedOutfit = null;
        if (outfit.Outfit.Garments.Count == 0)
        {
            StatusText.Text = "Add clothing components to this hanger first.";
            return;
        }
        DragDrop.DoDragDrop((DependencyObject)sender, new DataObject(AvatarOutfitDragFormats.Outfit, AvatarWardrobeStore.Snapshot(outfit.Outfit)), DragDropEffects.Copy);
    }
    private bool ShouldDrag(MouseEventArgs e)
    {
        Point current = e.GetPosition(this);
        return e.LeftButton == MouseButtonState.Pressed && (Math.Abs(current.X - dragStart.X) >= SystemParameters.MinimumHorizontalDragDistance || Math.Abs(current.Y - dragStart.Y) >= SystemParameters.MinimumVerticalDragDistance);
    }
    private void Outfit_DragOver(object sender, DragEventArgs e)
    {
        e.Effects = e.Data.GetDataPresent(AvatarOutfitDragFormats.Garment) ? DragDropEffects.Copy : DragDropEffects.None;
        e.Handled = true;
    }
    private void Outfit_Drop(object sender, DragEventArgs e)
    {
        e.Effects = DragDropEffects.None;
        RunAction(() =>
        {
            if ((sender as FrameworkElement)?.DataContext is OutfitEditorViewModel outfit
                && e.Data.GetData(AvatarOutfitDragFormats.Garment) is AvatarGarment garment)
            {
                AddGarment(outfit, garment);
                e.Effects = DragDropEffects.Copy;
            }
        });
        e.Handled = true;
    }
    private void RunAction(Action action)
    {
        try
        {
            action();
        }
        catch (Exception exception) when (exception is IOException or JsonException or ArgumentException or InvalidOperationException or UnauthorizedAccessException)
        {
            StatusText.Text = exception.Message;
        }
    }
}
