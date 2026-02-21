using System;
using System.Collections.Generic;
using System.Text;

namespace CatoriCity2025WPF.Objects.DragDrop
{
    public class LandscapeToCanvasDropHandler:IDropHandler
    {
        public void HandleDrop(UIElement dragged, UIElement target)
        {
            // Cast to your actual control types
            var landscape = dragged as LandscapeObjectControl;
            var canvas = target as Canvas;

            if (landscape == null || canvas == null)
                return;

            // Example: snap the person to the store's position
          

            // Or trigger game logic:
            // store.AddPerson(person);
        }
    }
}
