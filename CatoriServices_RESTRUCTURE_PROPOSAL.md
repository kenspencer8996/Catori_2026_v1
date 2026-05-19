# CatoriServices Restructure Proposal

Purpose: prospective file moves only. Edit this file as the final cut list before implementation.

Implementation notes:
- Keep class names unchanged unless a separate rename is explicitly listed.
- Preserve existing namespaces on the first pass unless namespace cleanup is requested after the move.
- Keep repository/entity pairs in parallel functional folders where possible.
- SQL scripts can be grouped, but moving embedded scripts may require updating copy-to-output paths or initializer paths.

## Proposed Top-Level Shape

```text
CatoriServices/
  Data/Stores/
  Database/Scripts/{Location,Manufacturing,RobotDesigner,Migrations}/
  Examples/Manufacturing/
  Objects/
    Core/
    Entities/{Banking,Business,City,House,Images,Locations,Manufacturing,People,Police,Robots,Settings,Stores,Treasure,Shared}/
    database/{Banking,Business,Core,House,Images,Locations,Manufacturing,People,Police,Robots,Settings,Stores,Treasure}/
    Services/Banking/
```

## Objects/database

### Banking
- `Objects/database/BankRepository.cs` -> `Objects/database/Banking/BankRepository.cs`
- `Objects/database/BankCustomerFundsRepository.cs` -> `Objects/database/Banking/BankCustomerFundsRepository.cs`
- `Objects/database/DepositRepository.cs` -> `Objects/database/Banking/DepositRepository.cs`

### Business
- `Objects/database/BusinessRepository.cs` -> `Objects/database/Business/BusinessRepository.cs`

### Core Database Infrastructure
- `Objects/database/AdoNetHelper.cs` -> `Objects/database/Core/AdoNetHelper.cs`
- `Objects/database/DatabaseHelper.cs` -> `Objects/database/Core/DatabaseHelper.cs`
- `Objects/database/LoggingRepository.cs` -> `Objects/database/Core/LoggingRepository.cs`
- `Objects/database/SQLiteConnection.cs` -> `Objects/database/Core/SQLiteConnection.cs`
- `Objects/database/SqlHelper.cs` -> `Objects/database/Core/SqlHelper.cs`

### House / City Objects
### House
- `Objects/database/HouseRepository.cs` -> `Objects/database/House/HouseRepository.cs`

### Images
- `Objects/database/ImageRepository.cs` -> `Objects/database/Images/ImageRepository.cs`

### Locations
- `Objects/database/LandscapeObjectRepository.cs` -> `Objects/database/House/LandscapeObjectRepository.cs`
- `Objects/database/LocationRepository.cs` -> `Objects/database/Locations/LocationRepository.cs`
- `Objects/database/LocationInventoryRepository.cs` -> `Objects/database/Locations/LocationInventoryRepository.cs`
- `Objects/database/LocationLayoutDatabaseInitializer.cs` -> `Objects/database/Locations/LocationLayoutDatabaseInitializer.cs`
- `Objects/database/LocationLayoutItemRepository.cs` -> `Objects/database/Locations/LocationLayoutItemRepository.cs`
- `Objects/database/LocationLayoutMachineRepository.cs` -> `Objects/database/Locations/LocationLayoutMachineRepository.cs`
- `Objects/database/LocationLayoutPointRepository.cs` -> `Objects/database/Locations/LocationLayoutPointRepository.cs`
- `Objects/database/LocationPartRouteRepository.cs` -> `Objects/database/Locations/LocationPartRouteRepository.cs`
- `Objects/database/ShelfLocationRepository.cs` -> `Objects/database/Locations/ShelfLocationRepository.cs`

### Manufacturing / Products
- `Objects/database/BillOfMaterialsRepository.cs` -> `Objects/database/Manufacturing/BillOfMaterialsRepository.cs`
- `Objects/database/ComponentRepository.cs` -> `Objects/database/Manufacturing/ComponentRepository.cs`
- `Objects/database/MachineRepository.cs` -> `Objects/database/Manufacturing/MachineRepository.cs`
- `Objects/database/MachineTypeRepository.cs` -> `Objects/database/Manufacturing/MachineTypeRepository.cs`
- `Objects/database/ManufacturingDatabaseInitializer.cs` -> `Objects/database/Manufacturing/ManufacturingDatabaseInitializer.cs`
- `Objects/database/ProductRepository.cs` -> `Objects/database/Manufacturing/ProductRepository.cs`

### People
- `Objects/database/PersonRepository.cs` -> `Objects/database/People/PersonRepository.cs`
- `Objects/database/PersonImageRepository.cs` -> `Objects/database/People/PersonImageRepository.cs`
- `Objects/database/PersonProductsOwnedRepository.cs` -> `Objects/database/People/PersonProductsOwnedRepository.cs`

### Police
- `Objects/database/PoliceCarRepository.cs` -> `Objects/database/Police/PoliceCarRepository.cs`

### Robots
- `Objects/database/RobotDesignerRepository.cs` -> `Objects/database/Robots/RobotDesignerRepository.cs`
- `Objects/database/RobotPoseRepository.cs` -> `Objects/database/Robots/RobotPoseRepository.cs`

### Settings
- `Objects/database/SettingsRepository.cs` -> `Objects/database/Settings/SettingsRepository.cs`

### Stores
- `Data/ShopItemRepository.cs` -> `Objects/database/Stores/ShopItemRepository.cs`

### Treasure / Learning
- `Objects/database/LearnedStepRepository.cs` -> `Objects/database/Treasure/LearnedStepRepository.cs`

## Objects/Entities

### Banking
- `BankEntity.cs`, `BankCustomerFundsEntity.cs`, `DepositEntity.cs` -> `Objects/Entities/Banking/`

### Business
- `BusinessEntity.cs`, `BusinessTypeEnum.cs` -> `Objects/Entities/Business/`

### City / Navigation
- `ApproqchPoitsEnum.cs`, `LoadStreetsEventArgs.cs`, `MapPositionEntity.cs`, `PathTypeEnum.cs`, `PositionsEWNSEnum.cs`, `SkiaMapperEventArgs.cs`, `StreetsModel.cs`, `StreetNavigationEntity.cs`, `StreetTraverseEnum.cs` -> `Objects/Entities/City/`

### House / Buildings
- `BuildingFacingEnum.cs`, `BuildingTypeEnum.cs`, `HouseEntity.cs`, `LotEntity.cs`, `LotPositionEnum.cs`, `LotSizeEnum.cs`, `ResidenceEntity.cs` -> `Objects/Entities/House/`

### Images
- `ImageContentEnum.cs`, `ImageDetailEntity.cs`, `ImageEnum.cs`, `ImageTypeEntity.cs` -> `Objects/Entities/Images/`

### Locations / Layout
- `InventoryEntity.cs`, `LocationAssemblyRobotEntity.cs`, `LocationEntity.cs`, `LocationEnum.cs`, `LocationLayoutEntity.cs`, `LocationLayoutItemEntity.cs`, `LocationLayoutMachineEntity.cs`, `LocationLayoutPointEntity.cs`, `LocationPartRouteEntity.cs`, `LocationPartRoutePointEntity.cs`, `LocationXYEntity.cs`, `ObjectLocationPathEntity.cs`, `ObjectLocationSizeInfoEntity.cs`, `ShelfLocationEntity.cs` -> `Objects/Entities/Locations/`

### Manufacturing / Products
- `BillOfMaterialsEntity.cs`, `ComponentEntity.cs`, `MachineEntity.cs`, `MachineTypeEntity.cs`, `ProductEntity.cs` -> `Objects/Entities/Manufacturing/`

### People
- `BadPersonImageEntity.cs`, `BadPersonImageTypeEnum.cs`, `BadPersonTypeEnum.cs`, `PersonEntity.cs`, `PersonEnum.cs`, `PersonImageEntity.cs`, `PersonImageStatusEnum.cs`, `PersonImageTypeEnum.cs`, `PersonProductsOwnedEntity.cs` -> `Objects/Entities/People/`

### Police / Vehicles
- `PoliceCarEntity.cs`, `VehicleTypeEnum.cs`, `VehicleEntity.cs` -> `Objects/Entities/Police/`

### Robots
- `RobotDesignerEntity.cs`, `RobotPoseEntity.cs` -> `Objects/Entities/Robots/`

### Settings / Core
- `SettingEntity.cs` -> `Objects/Entities/Settings/`
- `LogEntity.cs`, `SqliteColumnEntity.cs` -> `Objects/Entities/Shared/`

### Stores
- `ShopItemEntity.cs` -> `Objects/Entities/Stores/`

### Treasure / Learning
- `LearnedStepEntity.cs` -> `Objects/Entities/Treasure/`

## Objects/Services

### Banking
- `Objects/Services/DepositService.cs` -> `Objects/Services/Banking/DepositService.cs`

## Database/Scripts

### Location
- `Database/Scripts/CreateLocationLayoutTables.sql` -> `Database/Scripts/Location/CreateLocationLayoutTables.sql`
- `Database/Scripts/MergeLocationInteriorIntoLocation.sql` -> `Database/Scripts/Location/MergeLocationInteriorIntoLocation.sql`

### Manufacturing
- `Database/Scripts/CreateManufacturingTables.sql` -> `Database/Scripts/Manufacturing/CreateManufacturingTables.sql`

### Robot Designer
- `Database/Scripts/CreateRobotDesignerTables.sql` -> `Database/Scripts/RobotDesigner/CreateRobotDesignerTables.sql`
- `Database/Scripts/RobotMachineConfig/*` -> `Database/Scripts/RobotDesigner/RobotMachineConfig/`

### Migrations
- `Database/Scripts/CheckFactoryLocationSchema.sql` -> `Database/Scripts/Migrations/CheckFactoryLocationSchema.sql`
- `Database/Scripts/RenameFactoryTablesToLocation.sql` -> `Database/Scripts/Migrations/RenameFactoryTablesToLocation.sql`
- `Database/Scripts/RenameFactoryTablesToLocation_ColumnsAlreadyRenamed.sql` -> `Database/Scripts/Migrations/RenameFactoryTablesToLocation_ColumnsAlreadyRenamed.sql`
- `Database/Scripts/RecoverPartialFactoryToLocationMigration.sql` -> `Database/Scripts/Migrations/RecoverPartialFactoryToLocationMigration.sql`
- `Database/Scripts/RenameRobotDesignerPoseToRobotPose.sql` -> `Database/Scripts/Migrations/RenameRobotDesignerPoseToRobotPose.sql`

## Core Objects

- `Objects/Convertors.cs` -> `Objects/Core/Convertors.cs`
- `Objects/cLogger.cs` -> `Objects/Core/cLogger.cs`
- `Objects/GlobalServices.cs` -> `Objects/Core/GlobalServices.cs`

## Examples

- `Examples/ManufacturingDatabaseExample.cs` -> `Examples/Manufacturing/ManufacturingDatabaseExample.cs`

## Items To Decide Before Implementation

- Should namespace declarations be updated to match the new functional folders in this same pass?yes
- Should `Data/ShopItemRepository.cs` remain under `Data`, or move into `Objects/database/Stores` with the other repositories?repositories
- Should database scripts move now, or stay put to avoid changing initializer copy/output behavior?
- Should typo names be corrected during this restructure, such as `SqliteColumnEntity`, `BuildingTypeEnum`, `LotSizeEnum`, and `VehicleTypeEnum`?yes
- Should `LocationInventory` be considered a `Locations` feature or a `Manufacturing` feature?Locations

