using System.Windows.Media.Imaging;
using System.Windows.Threading;

namespace CatoriCity2025WPF.Objects
{
    public class UIUtility
    {

        private static string SavedClipboard = "";
        public static bool CheckMouseMoveForDrag(Point current, Point startposition)
        {
            bool result = false;
            double xoffset = Math.Abs(current.X - startposition.X);
            double yoffset = Math.Abs(current.Y - startposition.Y);

            if (xoffset > SystemParameters.MinimumHorizontalDragDistance ||
               yoffset > SystemParameters.MinimumVerticalDragDistance)
            {
                result = true;
            }
            return result;
        }
        /// <summary>
        /// Processes all pending UI events in the Dispatcher queue.
        /// </summary>
        public static void DoEvents()
        {
            // Create a new frame
            DispatcherFrame frame = new DispatcherFrame();

            // Schedule exit from the frame after all current events are processed
            Dispatcher.CurrentDispatcher.BeginInvoke(
                DispatcherPriority.Background,
                new DispatcherOperationCallback(ExitFrame), frame);

            // Process events
            Dispatcher.PushFrame(frame);
        }
        public static void SaveToClipboard(string item)
        {
            SavedClipboard = Clipboard.GetText();
            Clipboard.SetText(item);
        }
        public static string GetFromClipboard()
        {
            string clip = Clipboard.GetText();
            Clipboard.SetText(SavedClipboard);
            return clip; ; 
        }
        public static IEnumerable<T> FindChildrenOfType<T>(DependencyObject parent) where T : DependencyObject
        {
            if (parent == null) yield break;

            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(parent); i++)
            {
                var child = VisualTreeHelper.GetChild(parent, i);

                if (child is T typedChild)
                    yield return typedChild;

                foreach (var descendant in FindChildrenOfType<T>(child))
                    yield return descendant;
            }
        }
        private static object ExitFrame(object f)
        {
            ((DispatcherFrame)f).Continue = false;
            return null;
        }
        public static Image GetImageControl(string path,
            double HeightRequest, double WidthRequest,
            int zindex)
        {
            Image thisImage = new Image();
            try
            {
                if (path != null && path != "")
                {
                    thisImage.Height = HeightRequest;
                    thisImage.Width = WidthRequest;

                    // Fix: Convert the string path to an ImageSource using BitmapImage  
                    BitmapImage bitmap = new BitmapImage();
                    bitmap.BeginInit();
                    bitmap.UriSource = new System.Uri(path, System.UriKind.RelativeOrAbsolute);
                    bitmap.EndInit();
                    thisImage.Source = bitmap;
                }
            }
            catch (Exception ex)
            {
                cLogger.Log("UIUtility.GetImageControl exception:" + ex.Message);
            }

            return thisImage;
        }
        public static BitmapImage GetImageBitmap(string path)
        {
            BitmapImage bitmap = new BitmapImage();
            try
            {
                if (path != null && path != "")
                {

                    // Fix: Convert the string path to an ImageSource using BitmapImage  
                    bitmap.BeginInit();
                    bitmap.UriSource = new System.Uri(path, System.UriKind.RelativeOrAbsolute);
                    bitmap.EndInit();
                }
            }
            catch (Exception ex)
            {

                MessageBoxResult result = MessageBox.Show("UIUtility.GetImageControl exception:" + ex.Message, "Exception", MessageBoxButton.OK);

            }

            return bitmap;
        }
        internal static Brush GetSolidColorBrush(string screenBackgroundColor)
        {
            Brush brush = new SolidColorBrush((Color)ColorConverter.ConvertFromString(screenBackgroundColor));
            return brush;
        }
    }
}
