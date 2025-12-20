using CityAppServices.Objects.Entities;
using System.IO;

namespace CatoriCity2025WPF.Objects
{
    internal class ResourceHelperWin
    {
        public static void GetImagesAndLoadGlobalLists()
        {
            string path = AppDomain.CurrentDomain.BaseDirectory;
            string[] strings = Directory.GetFiles(path);
            foreach (string filenamewpath in strings)
            {
                try
                {

                    ImageTypeEntity imageType = new ImageTypeEntity();
                    string filename = Path.GetFileName(filenamewpath);
                    string[] parts = filename.Split('_');
                    string ext = Path.GetExtension(filenamewpath);
                    if (parts.Length > 1
                        && filename.StartsWith("appicon") == false
                        && (ext == "svg" || ext == "png"))
                    {
                        imageType.Name = filename;
                        imageType.number = Convert.ToInt32(parts[1]);
                        if (parts.Length > 2)
                            imageType.lastpart = parts[2];
                        if (filename.StartsWith("auto"))
                        {
                            imageType.Imagetype = ImageContentEnum.vehicle;
                            GlobalStuff.VechileImageList.Add(imageType);
                        }
                        else if (filename.StartsWith("badguy"))
                        {
                            imageType.Imagetype = ImageContentEnum.badguy;
                            GlobalStuff.BadguyImageList.Add(imageType);
                        }
                        else if (filename.StartsWith("bank"))
                        {
                            imageType.Imagetype = ImageContentEnum.bank;
                            GlobalStuff.FinancialImageList.Add(imageType);
                        }
                        else if (filename.StartsWith("carlot"))
                        {
                            imageType.Imagetype = ImageContentEnum.carlot;
                            GlobalStuff.CarlotImageList.Add(imageType);
                        }
                        else if (filename.StartsWith("classroom"))
                        {
                            imageType.Imagetype = ImageContentEnum.carlot;
                            GlobalStuff.ClassroomImageList.Add(imageType);
                        }
                        else if (filename.StartsWith("factory"))
                        {
                            imageType.Imagetype = ImageContentEnum.factory;
                            GlobalStuff.FactoryImageList.Add(imageType);
                        }
                        else if (filename.StartsWith("garage"))
                        {
                            imageType.Imagetype = ImageContentEnum.garage;
                            GlobalStuff.GarageImageList.Add(imageType);
                        }
                        else if (filename.StartsWith("girl"))
                        {
                            imageType.Imagetype = ImageContentEnum.girl;
                            GlobalStuff.PersonImageList.Add(imageType);
                        }
                        else if (filename.StartsWith("house"))
                        {
                            imageType.Imagetype = ImageContentEnum.house;
                            GlobalStuff.HouseImageList.Add(imageType);
                        }
                        else if (filename.StartsWith("kitchen"))
                        {
                            imageType.Imagetype = ImageContentEnum.room;
                            GlobalStuff.RoomImageList.Add(imageType);
                        }
                        else if (filename.StartsWith("living"))
                        {
                            imageType.Imagetype = ImageContentEnum.carlot;
                            GlobalStuff.RoomImageList.Add(imageType);
                        }
                        else if (filename.StartsWith("man"))
                        {
                            imageType.Imagetype = ImageContentEnum.man;
                            GlobalStuff.PersonImageList.Add(imageType);
                        }
                        else if (filename.StartsWith("pet"))
                        {
                            imageType.Imagetype = ImageContentEnum.store;
                            GlobalStuff.RetailImageList.Add(imageType);
                        }



                    }
                }
                catch (Exception ex)
                {

                }

            }
        }

    }
}
