namespace CatoriCity2025WPF.Objects
{
    internal class Imagehelper
    {
       
        internal static string GetImagePath(string filename)
        {
            string thisImage = "";
            try
            {
                if (filename != null && filename.StartsWith(GlobalStuff.ImageFolder) == false)
                {
                    try
                    {
                        filename = System.IO.Path.Combine(GlobalStuff.ImageFolder, filename);
                        thisImage = filename;
                    }
                    catch (Exception)
                    {
                        throw;
                    }
                }
            }
            catch (Exception ex)
            {

                throw;
            }
            if (thisImage != null && thisImage != "")
                return thisImage;
            else
                return filename;
        }
    }
}
