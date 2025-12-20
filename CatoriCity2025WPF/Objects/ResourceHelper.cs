namespace CatoriCity2025WPF.Objects
{
    internal class ResourceHelper
    {
        public static void GetImages()
        {

#if ANDROID

#elif IOS
            // results =  ResourceHelperIOS.GetImages();
#elif WINDOWS
            //  ResourceHelperWin.GetImagesAndLoadGlobalLists();
#endif

        }

    }
}
