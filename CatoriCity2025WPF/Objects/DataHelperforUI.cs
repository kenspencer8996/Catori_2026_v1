using CityAppServices.Objects.Entities;

namespace CatoriCity2025WPF.Objects
{
    internal class DataHelperforUI
    {
        public async static void LoadDB()
        {
            if (GlobalStuff.Images.Count() == 0)
                GlobalStuff.Images = await GlobalStuff.imageService.GetImagesAsync();
            if (GlobalStuff.Businesses.Count() == 0)
                GlobalStuff.Businesses = await GlobalStuff.businessService.GetBusinesssAsync();
            if (GlobalStuff.AllPersons.Count() == 0)
                GlobalStuff.AllPersons = await GlobalStuff.personService.GetPersonsAsync();
            if (GlobalStuff.PersonImages.Count() == 0)
                GlobalStuff.PersonImages = await GlobalStuff.personimageService.GetPersonImagesAsync();
            //if (GlobalStuff.PersonImages.Count() == 0)
            //    GlobalStuff.Settings = GlobalStuff.settingService.GetSetting();
 
            var foundhousesimages = from i in GlobalStuff.Images
                                    where i.ImageRole == ImageEnum.house
                                    select i;
            foreach (var image in foundhousesimages)
            {
                var foundhouses = from h in GlobalStuff.Houses
                                  where h.HouseImageFileName == ""
                                  select h;
                if (foundhouses.Any())
                {
                    foundhouses.First().HouseImageFileName = image.FilePath;
                    image.IsUsed = true;

                }

            }
            var foundpoliceimages = from i in GlobalStuff.Images
                                    where i.ImageRole == ImageEnum.policestation
                                    && i.IsUsed == false
                                    select i;
            foreach (var image in foundpoliceimages)
            {
                var foundhouses = from h in GlobalStuff.Houses
                                  where h.HouseImageFileName == ""
                                  select h;
                if (foundhouses.Any())
                {
                    foundhouses.First().HouseImageFileName = image.FilePath;
                    image.IsUsed = true;

                }

            }
            var foundfactoryimages = from i in GlobalStuff.Images
                                     where i.ImageRole == ImageEnum.factory
                                     && i.IsUsed == false
                                     select i;
            foreach (var image in foundfactoryimages)
            {
                var foundhouses = from h in GlobalStuff.Houses
                                  where h.HouseImageFileName == ""
                                  select h;
                if (foundhouses.Any())
                {
                    foundhouses.First().HouseImageFileName = image.FilePath;
                    image.IsUsed = true;

                }
            }
            var foundfinancialimages = from i in GlobalStuff.Images
                                       where i.ImageRole == ImageEnum.bank
                                       && i.IsUsed == false
                                       select i;
            foreach (var image in foundfinancialimages)
            {
                var foundbank = from h in GlobalStuff.Businesses
                                where h.ImageName == ""
                                select h;
                if (foundbank.Any())
                {
                    foundbank.First().ImageName = image.FilePath;
                    image.IsUsed = true;

                }
            }
            foreach (var item in GlobalStuff.AllPersons)
            {
                //item.CurrentImage
            }
        }
    }
}
