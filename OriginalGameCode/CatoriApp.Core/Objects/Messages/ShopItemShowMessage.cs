namespace CatoriApp.Core.Objects.Messages
{
    public class ShopItemShowMessage
    {
        public int PersistId { get; internal set; }
        public object PersonId { get; internal set; }
        public PersonViewModel Model { get; internal set; }
    }
}


