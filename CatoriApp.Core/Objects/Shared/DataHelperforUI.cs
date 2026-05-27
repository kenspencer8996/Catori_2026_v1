namespace CatoriApp.Core.Objects.Shared
{
    internal class DataHelperforUI
    {
        public async static void LoadDB()
        {
            if (CityScapeGlobal.Images.Count() == 0)
                CityScapeGlobal.Images = await CityScapeGlobal.imageService.GetImagesAsync();
            if (CityScapeGlobal.Businesses.Count() == 0)
                CityScapeGlobal.Businesses = await CityScapeGlobal.businessService.GetBusinesssAsync();
            if (GlobalAllApps.AllPersons.Count() == 0)
                GlobalAllApps.AllPersons = await CityScapeGlobal.personService.GetPersonsAsync();
            if (CityScapeGlobal.PersonImages.Count() == 0)
                CityScapeGlobal.PersonImages = await CityScapeGlobal.personimageService.GetPersonImagesAsync();
            //if (GlobalStuff.PersonImages.Count() == 0)
            //    GlobalStuff.Settings = GlobalStuff.settingService.GetSetting();
 
            var foundhousesimages = from i in CityScapeGlobal.Images
                                    where i.ImageRole == ImageEnum.house
                                    select i;
            foreach (var image in foundhousesimages)
            {
                var foundhouses = from h in CityScapeGlobal.Houses
                                  where h.HouseImageFileName == ""
                                  select h;
                if (foundhouses.Any())
                {
                    foundhouses.First().HouseImageFileName = image.FilePath;
                    image.IsUsed = true;

                }

            }
            var foundpoliceimages = from i in CityScapeGlobal.Images
                                    where i.ImageRole == ImageEnum.policestation
                                    && i.IsUsed == false
                                    select i;
            foreach (var image in foundpoliceimages)
            {
                var foundhouses = from h in CityScapeGlobal.Houses
                                  where h.HouseImageFileName == ""
                                  select h;
                if (foundhouses.Any())
                {
                    foundhouses.First().HouseImageFileName = image.FilePath;
                    image.IsUsed = true;

                }

            }
            var foundlocationimages = from i in CityScapeGlobal.Images
                                     where i.ImageRole == ImageEnum.location
                                     && i.IsUsed == false
                                     select i;
            foreach (var image in foundlocationimages)
            {
                var foundhouses = from h in CityScapeGlobal.Houses
                                  where h.HouseImageFileName == ""
                                  select h;
                if (foundhouses.Any())
                {
                    foundhouses.First().HouseImageFileName = image.FilePath;
                    image.IsUsed = true;

                }
            }
            var foundfinancialimages = from i in CityScapeGlobal.Images
                                       where i.ImageRole == ImageEnum.bank
                                       && i.IsUsed == false
                                       select i;
            foreach (var image in foundfinancialimages)
            {
                var foundbank = from h in CityScapeGlobal.Businesses
                                where h.ImageName == ""
                                select h;
                if (foundbank.Any())
                {
                    foundbank.First().ImageName = image.FilePath;
                    image.IsUsed = true;

                }
            }
            foreach (var item in GlobalAllApps.AllPersons)
            {
                //item.CurrentImage
            }
        }
    }
}



