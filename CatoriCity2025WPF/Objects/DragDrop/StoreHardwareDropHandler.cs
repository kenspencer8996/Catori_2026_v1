public class StoreHardwareDropHandler : IDropHandler
{
    public void HandleDrop(UIElement dragged, UIElement target)
    {
        // Cast to your actual control types
        var person = dragged as PersonControl;
        var store = target as StoreHardwareControl;

        if (person == null || store == null)
            return;

        // Example: snap the person to the store's position
        double storeX = Canvas.GetLeft(store);
        double storeY = Canvas.GetTop(store);

        // Offset so the person appears in front of the store
        //Canvas.SetLeft(person, storeX + 20);
        //Canvas.SetTop(person, storeY - 40);
        person.Opacity = 0; //hide the person as they are now "inside" the store

        // Or trigger game logic:
        store.AddPerson(person._person);
    }
}
