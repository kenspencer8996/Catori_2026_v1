using System.Text.Json.Serialization;

namespace CatoriUCLibrary.Objects.PersonItems;

public sealed class PersonItem
{
    public string Id { get; set; } = Guid.NewGuid().ToString("N");
    public string ItemName { get; set; } = "Item";
    public string ImagePath { get; set; } = "";
    public decimal Price { get; set; }
    public bool IsForSale { get; set; }
    public bool IsContainer { get; set; }
    public int Capacity { get; set; }
    [JsonObjectCreationHandling(JsonObjectCreationHandling.Populate)]
    public PersonItemInventory Contents { get; }
    [JsonIgnore]
    public PersonItemInventory? Owner { get; internal set; }
    public event EventHandler? OwnershipChanged;
    public PersonItem()
    {
        Contents = new PersonItemInventory(this);
    }
    internal void NotifyOwnershipChanged()
    {
        OwnershipChanged?.Invoke(this, EventArgs.Empty);
    }
}
