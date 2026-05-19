using CatoriApp.Objects;
using CatoriApp.Viewmodels;
using CatoriApp.ViewModels;
using CatoriApp.Views.Controls;
using CatoriServices.Objects;
using CityAppServices.Objects.Entities;
using System.IO;

namespace CatoriApp.Objects
{
    public class ImageFileHelper
    {
        //public static List<HouseViewModel> GetHouses()
        //{
        //    List<HouseViewModel> houseViewModels = new List<HouseViewModel>();
        //    List<HouseEntity> houses = new List<HouseEntity>();
        //    try
        //    {
        //        //string[] housefiles = System.IO.Directory.GetFiles(
        //        //    GlobalStuff.ImageFolder + "\\House*", "*.png");
        //        string searchPattern = "house*.*";
        //        string housesfolder = System.IO.Path.Combine(GlobalStuff.ImageFolder, "Houses");
        //        string[] housefiles = System.IO.Directory.GetFiles(housesfolder, searchPattern, SearchOption.TopDirectoryOnly).ToArray();
        //        cLogger.Log("housefiles count " + housefiles.Length.ToString());
        //        // Create a Random instance
        //        Random random = new Random();
        //        List<int> numbers = new List<int>();
        //        int housecounter = 0;
        //        cLogger.Log($"housefile ---------------"); // HouseControl: /CatoriApp; 100 100
        //        foreach (var housefile in housefiles)
        //        {
        //            numbers.Add(housecounter); // Add the index to the list
        //            housecounter++;
        //            cLogger.Log($"housefile: {housefile}"); // HouseControl: /CatoriApp; 100 100
        //        }
        //        Random rng = new Random();
        //        // Shuffle the list of numbers
        //        for (int ii = numbers.Count - 1; ii > 0; ii--)
        //        {
        //            int j = rng.Next(ii + 1);
        //            (numbers[ii], numbers[j]) = (numbers[j], numbers[ii]); // Swap elements
        //        }
        //        foreach (var number in numbers)
        //        {
        //            cLogger.Log($"number: {number}"); // HouseControl: /CatoriApp; 100 100
        //        }   

        //        // Randomly select items from housefiles
        //        int numberOfItemsToAdd = numbers.Count() ; // Random count of items to add
        //        int counter = 0;
        //        while (counter < numberOfItemsToAdd)
        //        {
        //            int index = numbers[counter];
        //            HouseEntity house = new HouseEntity
        //            {
        //                ImageFileName = housefiles[index]
        //            };
        //            cLogger.Log($"HouseEntity rand: {housefiles[index]}"); // HouseControl: /CatoriApp; 100 100
        //            houses.Add(house);
        //            counter++;
        //        }
        //        searchPattern = "living*.*";
        //        string[] livingroomfiles = System.IO.Directory.GetFiles(housesfolder, searchPattern, SearchOption.TopDirectoryOnly).ToArray();

        //        int livingroomcount = livingroomfiles.Length;
        //        int i = 0;
        //        foreach (var house in houses)
        //        {
        //            //house.ImageLivingRoomFileName = livingroomfiles[i];
        //            string housefilenameonly = System.IO.Path.GetFileNameWithoutExtension(house.ImageFileName);
        //            house.ImageLivingRoomFileName = GetFilenameForLivingRoomIfExists(housesfolder,housefilenameonly);
        //            string garagefilename = GetFilenameForGarageIfExists(housesfolder, housefilenameonly);  
        //            if (System.IO.File.Exists(garagefilename))
        //            {
        //                house.ImageGarageFileName = garagefilename;
        //            }
        //            else
        //                house.ImageGarageFileName = "";
        //            HouseViewModel model = new HouseViewModel();
        //            model.ToModel(house);
        //            houseViewModels.Add(model);
        //            if (i < livingroomcount)
        //            {
        //                i++;
        //            }
        //            else
        //                i = 0;
        //        }
        //        cLogger.Log($"end housefile ---------------"); // HouseControl: /CatoriApp; 100 100

        //    }
        //    catch (Exception)
        //    {

        //        throw;
        //    }

        //    return houseViewModels;
        //}
        //private static string GetFilenameForLivingRoomIfExists(string housesfolder,string houseImageFileName)
        //{
        //    string filename = System.IO.Path.GetFileNameWithoutExtension(houseImageFileName);
        //    string folder = housesfolder;
        //    string livingroomfilename = "";
        //    string[] files = System.IO.Directory.GetFiles(folder, "living*" + filename + ".*", SearchOption.TopDirectoryOnly);
        //    if (files.Length > 0)
        //    {
        //        livingroomfilename = files[0];
        //    }
        //    if (livingroomfilename == "")
        //    {
   
        //    }
        //    return livingroomfilename;
        //}
        //private static string GetFilenameForGarageIfExists(string housesfolder, string houseImageFileName)
        //{
        //    string filename = System.IO.Path.GetFileNameWithoutExtension(houseImageFileName);
        //    string folder = housesfolder;
        //    string livingroomfilename = "";
        //    string[] files = System.IO.Directory.GetFiles(folder, "garage*" + filename + ".*", SearchOption.TopDirectoryOnly);
        //    if (files.Length > 0)
        //    {
        //        livingroomfilename = files[0];
        //    }
        //    if (livingroomfilename == "")
        //    {

        //    }
        //    return livingroomfilename;
        //}
        private static List<HouseViewModel> SetRoomsInHouse(List<HouseViewModel> viewModels)
        {
            List<HouseViewModel> results = new List<HouseViewModel>();
            foreach (var item in viewModels)
            {
            }
            return results;
        }
        public static List<BankViewModel> GetBanks()
        {
            List<BankViewModel> viewModels = new List<BankViewModel>();
            try
            {
                //string[] housefiles = System.IO.Directory.GetFiles(
                //    GlobalStuff.ImageFolder + "\\House*", "*.png");
                string searchPattern = "bank*.*";
                string[] imagefiles = System.IO.Directory.GetFiles(GlobalAllApps.ImageFolder, searchPattern, SearchOption.TopDirectoryOnly).ToArray();
                cLogger.Log("bank count " + imagefiles.Length.ToString());
                // Create a Random instance
                Random random = new Random();
                List<int> numbers = new List<int>();
                int imagecounter = 0;
                foreach (var thisfile in imagefiles)
                {
                    numbers.Add(imagecounter); // Add the index to the list
                    imagecounter++;
                }
                Random rng = new Random();
                // Shuffle the list of numbers
                for (int ii = numbers.Count - 1; ii > 0; ii--)
                {
                    int j = rng.Next(ii + 1);
                    (numbers[ii], numbers[j]) = (numbers[j], numbers[ii]); // Swap elements
                }


                // Randomly select items from housefiles
                int numberOfItemsToAdd = numbers.Count(); // Random count of items to add
                int counter = 0;
                while (counter < numberOfItemsToAdd)
                {
                    int index = numbers[counter];
                    BankViewModel item = new BankViewModel();
                    item.BusinesskeyImageNameWOExtension = System.IO.Path.GetFileNameWithoutExtension(imagefiles[index]);
                    int i = imagefiles[index].IndexOf("_");
                    string filename = imagefiles[index].Substring(i+1);
                    item.Name = System.IO.Path.GetFileNameWithoutExtension( filename);
                    item.Name = item.Name.Replace("Bank", " Bank");
                    item.ImageFileName = imagefiles[index];
                    viewModels.Add(item);
                    counter++;
                }
            }
            catch (Exception ex)
            {

                throw;
            }

            return viewModels;
        }

 

        public static List<BusinessViewModel> GetLocations()
        {
            List<BusinessViewModel> viewModels = new List<BusinessViewModel>();
            List<BusinessEntity> business = new List<BusinessEntity>();
            try
            {
                //string[] housefiles = System.IO.Directory.GetFiles(
                //    GlobalStuff.ImageFolder + "\\House*", "*.png");
                string searchPattern = "location*.*";
                string[] imagefiles = System.IO.Directory.GetFiles(GlobalAllApps.ImageFolder, searchPattern, SearchOption.TopDirectoryOnly).ToArray();
                cLogger.Log("location count " + imagefiles.Length.ToString());
                // Create a Random instance
                Random random = new Random();
                List<int> numbers = new List<int>();
                int imagecounter = 0;
                foreach (var thisfile in imagefiles)
                {
                    numbers.Add(imagecounter); // Add the index to the list
                    imagecounter++;
                }
                Random rng = new Random();
                // Shuffle the list of numbers
                for (int ii = numbers.Count - 1; ii > 0; ii--)
                {
                    int j = rng.Next(ii + 1);
                    (numbers[ii], numbers[j]) = (numbers[j], numbers[ii]); // Swap elements
                }


                // Randomly select items from housefiles
                int numberOfItemsToAdd = numbers.Count(); // Random count of items to add
                int counter = 0;
                while (counter < numberOfItemsToAdd)
                {
                    int index = numbers[counter];
                    BusinessViewModel item = new BusinessViewModel();
                    item.SetImage(imagefiles[index]);
                    item.BusinessType = BusinessTypeEnum.Location;
                    viewModels.Add(item);
                    counter++;
                }

            }
            catch (Exception)
            {

                throw;
            }

            return viewModels;
        }
    }
}

