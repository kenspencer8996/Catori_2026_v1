using CatoriCity2025WPF.Objects;
using CatoriCity2025WPF.Viewmodels;
using CatoriCity2025WPF.ViewModels;
using CatoriCity2025WPF.Views.Controls;
using CatoriServices.Objects;
using CityAppServices.Objects.Entities;
using System.IO;

namespace CatoriCity2025WPF.Objects
{
    public class ImageFileHelper
    {
        public static List<HouseViewModel> GetHouses()
        {
            List<HouseViewModel> houseViewModels = new List<HouseViewModel>();
            List<HouseEntity> houses = new List<HouseEntity>();
            try
            {
                //string[] housefiles = System.IO.Directory.GetFiles(
                //    GlobalStuff.ImageFolder + "\\House*", "*.png");
                string searchPattern = "house*.*";
                string[] housefiles = System.IO.Directory.GetFiles(GlobalStuff.ImageFolder, searchPattern, SearchOption.TopDirectoryOnly).ToArray();
                cLogger.Log("housefiles count " + housefiles.Length.ToString());
                // Create a Random instance
                Random random = new Random();
                List<int> numbers = new List<int>();
                int housecounter = 0;
                cLogger.Log($"housefile ---------------"); // HouseControl: /CatoriCity2025WPF; 100 100
                foreach (var housefile in housefiles)
                {
                    numbers.Add(housecounter); // Add the index to the list
                    housecounter++;
                    cLogger.Log($"housefile: {housefile}"); // HouseControl: /CatoriCity2025WPF; 100 100
                }
                Random rng = new Random();
                // Shuffle the list of numbers
                for (int ii = numbers.Count - 1; ii > 0; ii--)
                {
                    int j = rng.Next(ii + 1);
                    (numbers[ii], numbers[j]) = (numbers[j], numbers[ii]); // Swap elements
                }
                foreach (var number in numbers)
                {
                    cLogger.Log($"number: {number}"); // HouseControl: /CatoriCity2025WPF; 100 100
                }   

                // Randomly select items from housefiles
                int numberOfItemsToAdd = numbers.Count() ; // Random count of items to add
                int counter = 0;
                while (counter < numberOfItemsToAdd)
                {
                    int index = numbers[counter];
                    HouseEntity house = new HouseEntity
                    {
                        ImageFileName = housefiles[index]
                    };
                    cLogger.Log($"HouseEntity rand: {housefiles[index]}"); // HouseControl: /CatoriCity2025WPF; 100 100
                    houses.Add(house);
                    counter++;
                }
                searchPattern = "living*.*";
                string[] livingroomfiles = System.IO.Directory.GetFiles(GlobalStuff.ImageFolder, searchPattern, SearchOption.TopDirectoryOnly).ToArray();

                int livingroomcount = livingroomfiles.Length;
                int i = 0;
                foreach (var house in houses)
                {
                    house.ImageLivingRoomFileName = livingroomfiles[i];
                    HouseViewModel model = new HouseViewModel();
                    model.ToModel(house);
                    houseViewModels.Add(model);
                    if (i < livingroomcount)
                    {
                        i++;
                    }
                    else
                        i = 0;
                }
                cLogger.Log($"end housefile ---------------"); // HouseControl: /CatoriCity2025WPF; 100 100

            }
            catch (Exception)
            {

                throw;
            }

            return houseViewModels;
        }
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
                string[] imagefiles = System.IO.Directory.GetFiles(GlobalStuff.ImageFolder, searchPattern, SearchOption.TopDirectoryOnly).ToArray();
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

 

        public static List<BusinessViewModel> GetFactories()
        {
            List<BusinessViewModel> viewModels = new List<BusinessViewModel>();
            List<BusinessEntity> business = new List<BusinessEntity>();
            try
            {
                //string[] housefiles = System.IO.Directory.GetFiles(
                //    GlobalStuff.ImageFolder + "\\House*", "*.png");
                string searchPattern = "factory*.*";
                string[] imagefiles = System.IO.Directory.GetFiles(GlobalStuff.ImageFolder, searchPattern, SearchOption.TopDirectoryOnly).ToArray();
                cLogger.Log("factory count " + imagefiles.Length.ToString());
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
                    item.BusinessType = BusinessTypeEnum.Factory;
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
