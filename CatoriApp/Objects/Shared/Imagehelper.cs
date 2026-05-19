namespace CatoriApp.Objects.Shared
{
    internal class Imagehelper
    {
       
        internal static string GetImagePath(string filename)
        {
            string thisImage = "";
            try
            {
                if (filename != null && filename.StartsWith(GlobalAllApps.ImageFolder) == false)
                {
                    try
                    {
                        filename = System.IO.Path.Combine(GlobalAllApps.ImageFolder, filename);
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


