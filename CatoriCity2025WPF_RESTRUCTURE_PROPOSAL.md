# CatoriApp Restructure Proposal

Purpose: prospective file moves only. Edit this file as the final cut list before implementation.

Implementation notes:
- Keep class names unchanged unless a separate rename is explicitly listed.
- Preserve existing namespaces on the first pass unless namespace cleanup is requested after the move.
- Move `.xaml` and `.xaml.cs` pairs together.
- After moving files, update XAML `x:Class`, project item paths if needed, and relative resource paths.

## Proposed Top-Level Shape

```text
CatoriApp/
  Controllers/{Banking,City,Factory,House,Manufacturing,People,Police,Robots,Stores,Treasure,Shared}/
  Objects/
    Arguments/
    DragDrop/
    DTOs/
    Messages/
    Services/{Banking,Business,House,Images,Locations,Manufacturing,People,Robots,Settings,Stores,Treasure}/
    Shared/
  ViewModels/{Banking,City,House,Images,Locations,Manufacturing,People,Police,Robots,Stores,Treasure,Shared}/
  Views/{Banking,City,Factory,House,Manufacturing,Settings,Stores,Treasure,Shared}/
```

## Controllers

### Banking
- `Controllers/BankListController.cs` -> `Controllers/Banking/BankListController.cs`
- `Controllers/BankEditController.cs` -> `Controllers/Banking/BankEditController.cs`
- `Controllers/BankDepositsController.cs` -> `Controllers/Banking/BankDepositsController.cs`
- `Controllers/FundsDetailViewController.cs` -> `Controllers/Banking/FundsDetailViewController.cs`

### City
- `Controllers/CityScapeViewController.cs` -> `Controllers/City/CityScapeViewController.cs`
- `Controllers/AnimationController.cs` -> `Controllers/City/AnimationController.cs`
- `Controllers/PhysicsDragController.cs` -> `Controllers/City/PhysicsDragController.cs`

### Factory / Location Interiors
- `Controllers/FactoryInterior_UController.cs` -> `Controllers/Factory/FactoryInterior_UController.cs`
- `Controllers/LocationInterior_1UCController.cs` -> `Controllers/Factory/LocationInterior_1UCController.cs`

### House
- `Controllers/HouseControlController.cs` -> `Controllers/House/HouseControlController.cs`
### Real Estate
- `Controllers/HousePurchaseViewController.cs` -> `Controllers/House/HousePurchaseViewController.cs`
- `Controllers/RealEstateInteriorViewController.cs` -> `Controllers/House/RealEstateInteriorViewController.cs`

### Manufacturing / Products
- `Controllers/ProductBuilderViewController.cs` -> `Controllers/Manufacturing/ProductBuilderViewController.cs`

### People
- `Controllers/BadPersonController.cs` -> `Controllers/People/BadPersonController.cs`

### Police
- `Controllers/CarPoliceContentController.cs` -> `Controllers/Police/CarPoliceContentController.cs`
- `Controllers/CarPoliceOffRoadControlController.cs` -> `Controllers/Police/CarPoliceOffRoadControlController.cs`

### Robots / Drones
- `Controllers/RobotUCController.cs` -> `Controllers/Robots/RobotUCController.cs`
- `Controllers/RobotPanelController.cs` -> `Controllers/Robots/RobotPanelController.cs`
- `Controllers/RobotCartControlController.cs` -> `Controllers/Robots/RobotCartControlController.cs`
- `Controllers/DroneDeliveryController.cs` -> `Controllers/Robots/DroneDeliveryController.cs`

### Stores
- `Controllers/StoreInteriorControllerBase.cs` -> `Controllers/Stores/StoreInteriorControllerBase.cs`
- `Controllers/StoreHardwareInteriorViewController.cs` -> `Controllers/Stores/StoreHardwareInteriorViewController.cs`
- `Controllers/POSUI_UCController.cs` -> `Controllers/Stores/POSUI_UCController.cs`

### Treasure
- `Controllers/TreasureFieldViewController.cs` -> `Controllers/Treasure/TreasureFieldViewController.cs`
- `Controllers/TreasureFieldLearnRunStepsController.cs` -> `Controllers/Treasure/TreasureFieldLearnRunStepsController.cs`
- `Controllers/Helpers/TreasureFieldViewControllerStepRunner.cs` -> `Controllers/Treasure/TreasureFieldViewControllerStepRunner.cs`

### Shared
- `Controllers/ControllerBase.cs` -> `Controllers/Shared/ControllerBase.cs`
- `Controllers/StartupViewController.cs` -> `Controllers/Shared/StartupViewController.cs`
- `Controllers/SettingsViewController.cs` -> `Controllers/Shared/SettingsViewController.cs`

## Objects/Services

### Banking
- `Objects/Services/BankService.cs` -> `Objects/Services/Banking/BankService.cs`
- `Objects/Services/BankCustomerFundsService.cs` -> `Objects/Services/Banking/BankCustomerFundsService.cs`
- `Objects/Services/DepositService.cs` -> `Objects/Services/Banking/DepositService.cs`

### Business
- `Objects/Services/BusinessService.cs` -> `Objects/Services/Business/BusinessService.cs`

### Locations / Factory
- `Objects/Services/LocationService.cs` -> `Objects/Services/Locations/LocationService.cs`
- `Objects/Services/LocationGeometryService.cs` -> `Objects/Services/Locations/LocationGeometryService.cs`
- `Objects/Services/LocationInventoryService.cs` -> `Objects/Services/Locations/LocationInventoryService.cs`
- `Objects/Services/LocationLayoutMachineService.cs` -> `Objects/Services/Locations/LocationLayoutMachineService.cs`
- `Objects/Services/LocationPartRouteService.cs` -> `Objects/Services/Locations/LocationPartRouteService.cs`

### House / City Objects
- `Objects/Services/HouseService.cs` -> `Objects/Services/House/HouseService.cs`
### Landscape
- `Objects/Services/LandscapeObjectService.cs` -> `Objects/Services/House/LandscapeObjectService.cs`

### Images
- `Objects/Services/ImageService.cs` -> `Objects/Services/Images/ImageService.cs`

### Manufacturing / Products
- `Objects/Services/ProductService.cs` -> `Objects/Services/Manufacturing/ProductService.cs`
- `Objects/Services/ComponentService.cs` -> `Objects/Services/Manufacturing/ComponentService.cs`
- `Objects/Services/BillOfMaterialsService.cs` -> `Objects/Services/Manufacturing/BillOfMaterialsService.cs`
- `Objects/Services/MachineService.cs` -> `Objects/Services/Manufacturing/MachineService.cs`
- `Objects/Services/MachineTypeService.cs` -> `Objects/Services/Manufacturing/MachineTypeService.cs`

### People
- `Objects/Services/PersonService.cs` -> `Objects/Services/People/PersonService.cs`
- `Objects/Services/PersonImageService.cs` -> `Objects/Services/People/PersonImageService.cs`
- `Objects/Services/PersonProductsOwnedService.cs` -> `Objects/Services/People/PersonProductsOwnedService.cs`

### Robots
- `Objects/Services/RobotDesignerService.cs` -> `Objects/Services/Robots/RobotDesignerService.cs`
- `Objects/Services/RobotDesignerSelectionService.cs` -> `Objects/Services/Robots/RobotDesignerSelectionService.cs`
- `Objects/Services/RobotPoseService.cs` -> `Objects/Services/Robots/RobotPoseService.cs`

### Settings
- `Objects/Services/SettingService.cs` -> `Objects/Services/Settings/SettingService.cs`

### Stores
- `Objects/Services/ShopItemService.cs` -> `Objects/Services/Stores/ShopItemService.cs`
- `Objects/Services/ShelfLocationService.cs` -> `Objects/Services/Locations/ShelfLocationService.cs`

### Treasure
- `Objects/Services/LearnedStepService.cs` -> `Objects/Services/Treasure/LearnedStepService.cs`

## ViewModels

### Banking
- `BankViewModel.cs`, `BankCustomerFundsViewModel.cs`, `DepositViewModel.cs`, `FundsViewModel.cs` -> `ViewModels/Banking/`

### City
- `AirportViewModel.cs`, `BusinessViewModel.cs`, `CityappViewmodel.cs`, `CityLayoutcontrolViewmodel.cs`, `CityscapeStreetsViewModel.cs`, `CityStreetMasterViewModel.cs`, `LandscapeObjectViewModel.cs`, `MainWindowViewModel.cs`, `PositionNavigationMasterModel.cs` -> `ViewModels/City/`

### House
- `HouseViewModel.cs`, `KitchenPageViewModel.cs`, `LivingRoomPageViewmodel.cs` -> `ViewModels/House/`

### Images
- `ImageViewModel.cs` -> `ViewModels/Images/`

### Locations / Factory
- `CampfireSpotLocationViewmodel.cs`, `InventoryItemViewModel.cs`, `LocationDesignerViewModel.cs`, `LocationLayoutItemViewModel.cs`, `LocationLayoutMachineViewModel.cs`, `LocationLayoutPointViewModel.cs`, `LocationPartRoutePointViewModel.cs`, `LocationPartRouteViewModel.cs`, `LocationViewModel.cs`, `ShelfLocationViewModel.cs` -> `ViewModels/Locations/`

### Manufacturing
- `BomItemViewModel.cs`, `ComponentViewModel.cs`, `MachineTypeViewModel.cs`, `MachineViewModel.cs`, `ManufacturingViewModel.cs`, `ProductViewModel.cs` -> `ViewModels/Manufacturing/`

### People
- `PersonImageViewModel.cs`, `PersonProductsOwnedViewModel.cs`, `PersonViewModel.cs`, `PersonViewModelBase.cs`, `RobberyMessageDetailViewModel.cs` -> `ViewModels/People/`

### Police
- `CarPoliceViewModel.cs`, `PoliceCarViewModel.cs`, `careventArg.cs` -> `ViewModels/Police/`

### Robots
- `RobotDesignerViewModel.cs`, `RobotPanelViewModel.cs`, `RobotPoseViewModel.cs` -> `ViewModels/Robots/`

### Stores
- `ShopItemViewModel.cs`, `ShoppingCartItemViewModel.cs` -> `ViewModels/Stores/`

### Treasure
- `LearnedStepModel.cs`, `TreasureFieldLearnRunStepsviewModel.cs` -> `ViewModels/Treasure/`

### Shared
- `BaseViewModel.cs`, `StatusViewModel.cs`, `ViewmodelBase.cs` -> `ViewModels/Shared/`

## Views

### Banking
- `Views/FundsDetailView.xaml(.cs)` -> `Views/Banking/`
- `Views/Controls/Bank/*` -> `Views/Controls/Banking/`
- `Views/Controls/BankDepositsView.xaml(.cs)` -> `Views/Controls/Banking/`

### City
- `Views/CityScapeView.xaml(.cs)` -> `Views/City/`
### Landscape
- `Views/Controls/Landscape/*` -> `Views/Controls/City/Landscape/`
- `Views/LandscapeObjectDetailView.xaml(.cs)` -> `Views/City/`

### Factory / Location Interiors
- `Views/FactoryView.xaml(.cs)` -> `Views/Factory/`
- `Views/Controls/Locations/Factory/*` -> keep under `Views/Controls/Locations/Factory/` for now

### House
- `Views/DetailHouseInsideView.xaml(.cs)`, `Views/HousePurchaseView.xaml(.cs)`, `Views/RealestateInteriorView.xaml(.cs)` -> `Views/House/`
- `Views/Controls/House/*` -> keep under `Views/Controls/House/`

### Manufacturing
- `Views/ProductBuilderView.xaml(.cs)` -> `Views/Manufacturing/`
- `Views/RobotDesigner/*` -> `Views/Manufacturing/RobotDesigner/`

### Settings / Startup
- `Views/SettingsView.xaml(.cs)`, `Views/SettingDetailView.xaml(.cs)` -> `Views/Settings/`
- `Views/StartupView.xaml(.cs)` -> `Views/Shared/`

### Stores
- `Views/StoreViews/*` -> `Views/Stores/`
- `Views/Controls/Stores/*` -> keep under `Views/Controls/Stores/`
- `Views/Controls/POSUI_UC.xaml(.cs)` -> `Views/Controls/Stores/`

### Treasure
- `Views/TreasureFieldView.xaml(.cs)` -> `Views/Treasure/`
- `Views/Controls/Treasure/*` -> keep under `Views/Controls/Treasure/`

### Shared Controls
- `Views/Controls/CommonControls/*` -> keep under `Views/Controls/CommonControls/`
- `Views/Controls/StatusControl1.xaml(.cs)`, `Views/Controls/LightPanel.xaml(.cs)`, `Views/Controls/TrashCanUC.xaml(.cs)` -> `Views/Controls/CommonControls/`

## Objects

### Leave Existing Subfolders
- `Objects/Arguments/*`
- `Objects/DragDrop/*`
- `Objects/DTOs/*`
- `Objects/Messages/*`
- `Objects/Extensions/*`

### Shared Utility Candidates
- Move general helpers/globals into `Objects/Shared/`: `AnimationHelper.cs`, `CityScapeGlobal.cs`, `CsvImportService.cs`, `DataHelperforUI.cs`, `DBLogger.cs`, `DefaultSampleDataHelper.cs`, `FileHelper.cs`, `GenericSerializer.cs`, `GeometryHelper.cs`, `GlobalAllApps.cs`, `GlobalCode.cs`, `GlobalGeo.cs`, `HelperStuff.cs`, `ImageFileHelper.cs`, `Imagehelper.cs`, `JsonSerializerHelper.cs`, `MathHelper.cs`, `ResourceHelper*.cs`, `ReturnForInsertMethods.cs`, `SampleData.cs`, `ShoppingCartUtility.cs`, `StorageFolder.cs`, `StreetHelper.cs`, `StreetLayoutHelper.cs`, `UIUtility.cs`.

## Items To Decide Before Implementation

- Should `Factory` UI folders remain named `Factory`, or move to `Locations` now? Factory should be  under Location\Factory.
- Should typo folders/files be corrected during this restructure, such as `Bank`, `SettingDetailView`, `CarPolice*`, and `RealEstateInteriorViewController`? yes
- Should namespaces be updated to match folders in this same pass, or should the first pass only move files and keep namespaces stable? yes
- Should `RobotDesigner` live under `Robots` or `Manufacturing`? Robots
- Should `LocationInventoryService` live under `Locations` or `Manufacturing`? Locations

