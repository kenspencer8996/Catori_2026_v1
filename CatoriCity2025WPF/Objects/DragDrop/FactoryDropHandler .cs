public class FactoryControlDropHandler : IDropHandler
{
    public void HandleDrop(UIElement dragged, UIElement target)
    {
        // Cast to your actual control types
        var person = dragged as PersonControl;
        var factory = target as FactoryControl;

        if (person == null || factory == null)
            return;

        // Example: snap the person to the store's position
        double factoryX = Canvas.GetLeft(factory);
        double factoryY = Canvas.GetTop(factory);

        person.Opacity = 0; //hide the person as they are now "inside" the factory

        // Or trigger game logic:
        factory.AddPerson(person._person);
    }
}
