using System.IO;
using System.Text.Json;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using CatoriUCLibrary.Objects.PersonItems;

namespace CatoriUCLibrary.Views;

public partial class ItemUC : UserControlWithProperties
{
    public CatoriUCLibrary.Objects.PersonItems.ItemAction Action { get; set; }
    private Point? _dragStart;
    private PersonItemInventory? _source;
    private bool _ownsSource;
    public static readonly DependencyProperty ItemProperty = DependencyProperty.Register(
        nameof(Item), typeof(PersonItem), typeof(ItemUC), new PropertyMetadata(null, ItemChanged));
    public PersonItem? Item
    {
        get => (PersonItem?)GetValue(ItemProperty);
        set => SetValue(ItemProperty, value);
    }
    public string ItemJson
    {
        get => JsonSerializer.Serialize(Item);
        set => Item = JsonSerializer.Deserialize<PersonItem>(value);
    }
    public string ItemName
    {
        get => Item?.ItemName ?? "Item";
        set
        {
            EnsureItem().ItemName = value;
            RefreshItem();
        }
    }
    public string ImagePath
    {
        get => Item?.ImagePath ?? "";
        set
        {
            EnsureItem().ImagePath = value;
            RefreshItem();
        }
    }
    public bool IsContainer
    {
        get => Item?.IsContainer ?? false;
        set
        {
            EnsureItem().IsContainer = value;
            RefreshItem();
        }
    }
    public int Capacity
    {
        get => Item?.Capacity ?? 0;
        set
        {
            if (value < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(value));
            }
            EnsureItem().Capacity = value;
        }
    }
    private PersonItem EnsureItem()
    {
        Item ??= new PersonItem();
        return Item;
    }
    public ItemUC()
    {
        InitializeComponent();
        Loaded += (_, _) => RefreshItem();
        Unloaded += (_, _) => UnsubscribeItem();
    }

    public bool TryReceiveItem(PersonItem incoming)
    {
        if (Item?.IsContainer != true || !Item.Contents.CanReceive(incoming))
        {
            return false;
        }
        Item.Contents.Receive(incoming);
        return true;
    }
    private static void ItemChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
    {
        var control = (ItemUC)sender;
        if (e.OldValue is PersonItem old)
        {
            old.OwnershipChanged -= control.OwnershipChanged;
        }
        control._source = null;
        control._ownsSource = e.NewValue is PersonItem { Owner: null };
        control.RefreshItem();
    }
    private void UnsubscribeItem()
    {
        if (Item != null)
        {
            Item.OwnershipChanged -= OwnershipChanged;
        }
    }
    private void RefreshItem()
    {
        ItemImage.Source = null;

        OpenPersonItemContentsMenuItem.IsEnabled = Item?.IsContainer == true;
        if (Item == null)
        {
            return;
        }
        Item.OwnershipChanged -= OwnershipChanged;
        Item.OwnershipChanged += OwnershipChanged;
        Item.Contents.RestoreOwnership();
        if (_ownsSource && _source == null)
        {
            _source = new PersonItemInventory();
            _source.Receive(Item);
        }
        if (!string.IsNullOrWhiteSpace(Item.ImagePath))
        {
            string path = Path.IsPathRooted(Item.ImagePath) ? Item.ImagePath : Path.Combine(AppContext.BaseDirectory, Item.ImagePath);
            if (File.Exists(path))
            {
                using var stream = File.OpenRead(path);
                var image = new BitmapImage();
                image.BeginInit();
                image.CacheOption = BitmapCacheOption.OnLoad;
                image.StreamSource = stream;
                image.EndInit();
                image.Freeze();
                ItemImage.Source = image;
            }
        }
        OwnershipChanged(this, EventArgs.Empty);
    }
    private void OwnershipChanged(object? sender, EventArgs e)
    {
        Visibility = _ownsSource && !ReferenceEquals(Item?.Owner, _source) ? Visibility.Collapsed : Visibility.Visible;
    }
    private void Item_MouseDown(object sender, MouseButtonEventArgs e)
    {
        _dragStart = e.GetPosition(this);
        e.Handled = true;
    }
    private void Item_MouseMove(object sender, MouseEventArgs e)
    {
        if (_dragStart is not Point start || e.LeftButton != MouseButtonState.Pressed || Item == null)
        {
            _dragStart = null;
            return;
        }
        Point current = e.GetPosition(this);
        if (Math.Abs(current.X - start.X) < SystemParameters.MinimumHorizontalDragDistance
            && Math.Abs(current.Y - start.Y) < SystemParameters.MinimumVerticalDragDistance)
        {
            return;
        }
        _dragStart = null;
        var preview = new ItemDragPreviewWindow(ItemImage.Source, this);
        GiveFeedbackEventHandler feedback = (_, _) => preview.MoveToCursor();
        GiveFeedback += feedback;
        try
        {
            preview.Show();
            preview.MoveToCursor();
            var data = PersonItemDrag.Create(Item);
            data.SetData(ItemDropTarget.DragSizeFormat, new Size(ActualWidth, ActualHeight));
            DragDrop.DoDragDrop(this, data, DragDropEffects.Move);
        }
        finally
        {
            GiveFeedback -= feedback;
            preview.Close();
        }
        e.Handled = true;
    }
    private void Item_DragOver(object sender, DragEventArgs e)
    {
        if (Item?.IsContainer != true)
        {
            return;
        }
        PersonItem? incoming = PersonItemDrag.Read(e.Data);
        e.Effects = incoming != null && Item?.IsContainer == true && Item.Contents.CanReceive(incoming)
            ? DragDropEffects.Move : DragDropEffects.None;
        e.Handled = true;
    }
    private void Item_Drop(object sender, DragEventArgs e)
    {
        if (Item?.IsContainer != true)
        {
            return;
        }
        PersonItem? incoming = PersonItemDrag.Read(e.Data);
        e.Effects = DragDropEffects.None;
        if (incoming != null && TryReceiveItem(incoming))
        {
            e.Effects = DragDropEffects.Move;
        }
        e.Handled = true;
    }
    private void OpenContents_Click(object sender, RoutedEventArgs e)
    {
        if (Item?.IsContainer != true)
        {
            return;
        }
        var window = new ItemContentsWindow(Item)
        {
            Owner = Window.GetWindow(this),
        };
        window.Show();
    }
}


