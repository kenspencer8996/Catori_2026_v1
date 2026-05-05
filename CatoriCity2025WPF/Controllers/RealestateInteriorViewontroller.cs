using CatoriCity2025WPF.Objects.Messages;
using CatoriCity2025WPF.Views;
using CommunityToolkit.Mvvm.Messaging;

namespace CatoriCity2025WPF.Controllers
{
    public class RealestateInteriorViewontroller
    {
        private readonly RealestateInteriorView _view;
        private readonly HouseService _houseService;
        private readonly PersonService _personService;
        PersonViewModel _catori;
        HouseViewModel _selectedHouse;
            public RealestateInteriorViewontroller(RealestateInteriorView view)
            {
                _view = view;
      
                _houseService = new HouseService();
                _personService = new PersonService();
            }

            public async Task LoadHouses()
            {
                try
                {
                    _catori = await _personService.GetPersonbyNameAsync("Catori");
                    
                    // Load houses owned by current player
                    var allHouses = _houseService.GetHouses();
                    var housesForSale   = from h in allHouses
                                        where h.ForSale == 1
                                        select h;   
                    _view.HouseComboBox.ItemsSource = housesForSale;
                    ClearHouseDetails();
                    cLogger.Log($"Loaded {allHouses.Count} houses");
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error loading houses: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    cLogger.Log($"LoadHouses error: {ex.Message}");
                }
            }

            public void OnHouseSelected(HouseViewModel house)
            {
                _selectedHouse = house;

                if (house == null)
                {
                    ClearHouseDetails();
                    return;
                }

                // Update house details
                _view.HouseNameText.Text = house.Name;
                _view.CurrentPriceText.Text = house.Price > 0 ? house.PriceFormatted : "Not set";

                // Load house image
                try
                {
                    _view.HouseImage.Source = UIUtility.GetImageControl(house.HouseImageFileName, 200, 200, 0).Source;
                }
                catch (Exception ex)
                {
                    cLogger.Log($"Error loading house image: {ex.Message}");
                }

                if (GlobalAllApps.CurrentPerson.Funds > house.Price)
                {
                    _view.StatusText.Text = "You can purchase this house";
                    _view.StatusText.Foreground = Brushes.Green;
                    _view.StatusText.FontWeight = FontWeights.Bold;
                    _view.BuyHouseButton.IsEnabled = true;
                }
                else
                {
                    _view.StatusText.Text = "You do not have enough money for this house.";
                    _view.StatusText.Foreground = Brushes.Gray;
                    _view.StatusText.FontWeight = FontWeights.Normal;
                    _view.BuyHouseButton.IsEnabled = false;
                }

                // Set current price in textbox if exists
                _view.SalePriceTextBox.Text = house.Price > 0 ? house.Price.ToString("F2") : "";
            }

        internal void BuyHouse()
        {
            _selectedHouse.ForSale = 0;
            _selectedHouse.OwnerName = GlobalAllApps.CurrentPerson.Name;
            _houseService.Upsert(_selectedHouse);
            var house = from h in CityScapeGlobal.Houses
                        where h.Name == _selectedHouse.Name
                        select h;
            GlobalAllApps.CurrentPerson.Funds -= _selectedHouse.Price;
            _personService.UpsertPerson(GlobalAllApps.CurrentPerson);
            WeakReferenceMessenger.Default.Send(new HouseSoldMessage(_selectedHouse));
        }

        private void ClearHouseDetails()
            {
                _view.HouseNameText.Text = "";
                _view.StatusText.Text = "";
                _view.CurrentPriceText.Text = "";
                _view.SalePriceTextBox.Text = "";
                _view.HouseImage.Source = null;
                _view.BuyHouseButton.IsEnabled = false;
            }
        }
    }