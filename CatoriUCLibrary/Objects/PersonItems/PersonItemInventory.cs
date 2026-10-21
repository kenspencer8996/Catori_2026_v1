using System.Collections.ObjectModel;
using System.Text.Json.Serialization;

namespace CatoriUCLibrary.Objects.PersonItems;

public sealed class PersonItemInventory
{
    [JsonObjectCreationHandling(JsonObjectCreationHandling.Populate)]
    public ObservableCollection<PersonItem> Items { get; } = [];
    [JsonIgnore]
    public PersonItem? Container { get; }
    public PersonItemInventory()
    {
    }
    public PersonItemInventory(PersonItem container)
    {
        Container = container;
    }
    public bool CanReceive(PersonItem item)
    {
        if (Container != null && (!Container.IsContainer || (Container.Capacity > 0 && Items.Count >= Container.Capacity && !Items.Contains(item))))
        {
            return false;
        }
        for (PersonItem? parent = Container; parent != null; parent = parent.Owner?.Container)
        {
            if (ReferenceEquals(parent, item))
            {
                return false;
            }
        }
        return true;
    }
    public void Receive(PersonItem item)
    {
        ArgumentNullException.ThrowIfNull(item);
        if (ReferenceEquals(item.Owner, this))
        {
            return;
        }
        if (!CanReceive(item))
        {
            throw new InvalidOperationException("This container is full or cannot accept that item.");
        }
        item.Owner?.Items.Remove(item);
        if (!Items.Contains(item))
        {
            Items.Add(item);
        }
        item.Owner = this;
        item.NotifyOwnershipChanged();
    }
    public void RestoreOwnership()
    {
        foreach (PersonItem item in Items.ToArray())
        {
            Receive(item);
            item.Contents.RestoreOwnership();
        }
    }
}
