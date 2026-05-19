using CatoriApp.Objects.Enums;
using System.IO;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
namespace CatoriApp.Objects.Services.Robots
{
    public class MachineLayoutDesignerSelectionService
    {
        public MachineLayoutDesignerWindow OpenDesignerForImage(
            string imagePath, Rect sourceSelection, MachineDesignerModeEnum mode)
        {
            var window = new MachineLayoutDesignerWindow(0, 
                imagePath, sourceSelection,mode);
            window.Show();
            return window;
        }

        public MachineLayoutDesignerWindow CaptureSelectionAndOpen(FrameworkElement source, 
            Rect selectionBounds, MachineDesignerModeEnum mode)
        {
            var imagePath = CaptureSelection(source, selectionBounds);
            return OpenDesignerForImage(imagePath, selectionBounds,mode);
        }

        public string CaptureSelection(FrameworkElement source, Rect selectionBounds)
        {
            if (source.ActualWidth <= 0 || source.ActualHeight <= 0)
                throw new InvalidOperationException("Source element must be loaded before capture.");

            var render = new RenderTargetBitmap(
                (int)Math.Ceiling(source.ActualWidth),
                (int)Math.Ceiling(source.ActualHeight),
                96,
                96,
                PixelFormats.Pbgra32);

            render.Render(source);

            var crop = new Int32Rect(
                Math.Max(0, (int)Math.Floor(selectionBounds.X)),
                Math.Max(0, (int)Math.Floor(selectionBounds.Y)),
                Math.Max(1, (int)Math.Ceiling(selectionBounds.Width)),
                Math.Max(1, (int)Math.Ceiling(selectionBounds.Height)));

            crop.Width = Math.Min(crop.Width, render.PixelWidth - crop.X);
            crop.Height = Math.Min(crop.Height, render.PixelHeight - crop.Y);

            var cropped = new CroppedBitmap(render, crop);
            var folder = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                "Catori",
                "MachineLayoutDesignerCaptures");

            Directory.CreateDirectory(folder);
            var filePath = Path.Combine(folder, "MachineLayoutDesignerCapture_" + DateTime.Now.ToString("yyyyMMdd_HHmmssfff") + ".png");

            var encoder = new PngBitmapEncoder();
            encoder.Frames.Add(BitmapFrame.Create(cropped));

            using var stream = File.Create(filePath);
            encoder.Save(stream);

            return filePath;
        }
    }
}



