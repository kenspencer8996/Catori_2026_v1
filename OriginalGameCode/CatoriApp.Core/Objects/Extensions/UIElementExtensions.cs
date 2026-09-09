using System;
using System.Collections.Generic;
using System.Text;
namespace CatoriApp.Core.Objects.Extensions
{
    public static class UIElementExtensions
    {

        /// <summary>
        /// Gets the top-left position of a UIElement relative to a specified ancestor after all transforms.
        /// </summary>
        public static Point GetTransformedPosition(this UIElement element, Visual relativeTo)
        {
            if (element == null || relativeTo == null)
                throw new ArgumentNullException();

            // Transform (0,0) from element's local space to the ancestor's coordinate space
            GeneralTransform transform = element.TransformToAncestor(relativeTo);
            return transform.Transform(new Point(0, 0));
        }

    }
}


