using System.Windows.Controls;

namespace CatoriCity2025WPF.Objects
{
    internal class UIHelpers
    {

        /// <summary>
        /// Finds the first parent of the specified type in the visual tree.
        /// </summary>
        public static T FindParent<T>(DependencyObject child) where T : DependencyObject
        {
            if (child == null) return null;

            // Get the immediate parent
            DependencyObject parentObject = VisualTreeHelper.GetParent(child);

            // If no parent, return null
            if (parentObject == null) return null;

            // Check if the parent matches the type
            if (parentObject is T parent)
            {
                return parent;
            }
            else
            {
                // Recursively search up the tree
                return FindParent<T>(parentObject);
            }
        }
    }
}
