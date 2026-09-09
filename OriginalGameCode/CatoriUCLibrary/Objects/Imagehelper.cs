namespace CatoriUCLibrary.Views.RobotArm;

internal static class Imagehelper
{
    public static string GetImagePath(string filename)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(filename);

        if (System.IO.Path.IsPathRooted(filename))
            return filename;

        string fileName = System.IO.Path.GetFileName(
            filename.Replace('/', System.IO.Path.DirectorySeparatorChar));
        return $"pack://application:,,,/CatoriUCLibrary;component/Images/{fileName}";
    }
}
