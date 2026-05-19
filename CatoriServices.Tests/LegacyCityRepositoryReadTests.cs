using CatoriServices.Objects.database;
using CatoriServices.Objects.Entities;

namespace CatoriServices.Tests;

[Collection(GlobalTestStateCollection.Name)]
public sealed class LegacyCityRepositoryReadTests
{
    private readonly GlobalTestState _globalState;

    public LegacyCityRepositoryReadTests(GlobalTestState globalState)
    {
        _globalState = globalState;
    }

    [Fact]
    public async Task BusinessRepository_reads_seeded_business()
    {
        using var db = new SqliteTestDatabase();
        await db.ExecuteScriptAsync("""
            CREATE TABLE Business (
                Name TEXT NOT NULL,
                EmployeePayHour NUMERIC NOT NULL,
                BusinessType INTEGER NOT NULL,
                ImageName TEXT NOT NULL
            );
            INSERT INTO Business (Name, EmployeePayHour, BusinessType, ImageName)
            VALUES ('LocationCo', 22, 5, 'location.png');
            """);
        _globalState.UseDatabase(db.DatabasePath);

        var business = await new BusinessRepository().GetBusinessbyNameAsync("LocationCo");

        Assert.Equal("LocationCo", business.Name);
        Assert.Equal(22m, business.EmployeePayHour);
        Assert.Equal(BusinessTypeEnum.Location, business.BusinessType);
        Assert.Equal("location.png", business.ImageName);
    }

    [Fact]
    public async Task DepositRepository_reads_seeded_deposit()
    {
        using var db = new SqliteTestDatabase();
        await db.ExecuteScriptAsync("""
            CREATE TABLE Deposit (
                DepositId INTEGER PRIMARY KEY,
                BankId INTEGER NOT NULL,
                PersonId INTEGER NOT NULL,
                Amount NUMERIC NOT NULL,
                businessname TEXT NOT NULL
            );
            INSERT INTO Deposit (DepositId, BankId, PersonId, Amount, businessname)
            VALUES (3, 4, 5, 99.75, 'LocationCo');
            """);
        _globalState.UseDatabase(db.DatabasePath);

        var deposit = await new DepositRepository().GetDepositByIdAsync(3);

        Assert.Equal(3, deposit.DepositId);
        Assert.Equal(4, deposit.BankId);
        Assert.Equal(5, deposit.PersonId);
        Assert.Equal(99.75m, deposit.Amount);
        Assert.Equal("LocationCo", deposit.BusinessName);
    }

    [Fact]
    public async Task PersonRepository_reads_seeded_person()
    {
        using var db = new SqliteTestDatabase();
        await db.ExecuteScriptAsync("""
            CREATE TABLE Person (
                PersonID INTEGER PRIMARY KEY,
                Name TEXT NOT NULL,
                ImagesFolder TEXT NOT NULL,
                PersonRole INTEGER NOT NULL,
                IsUser INTEGER NOT NULL,
                Funds NUMERIC NOT NULL,
                Active INTEGER NOT NULL,
                FileNameOptional TEXT NOT NULL
            );
            INSERT INTO Person (PersonID, Name, ImagesFolder, PersonRole, IsUser, Funds, Active, FileNameOptional)
            VALUES (8, 'Alex', 'people/alex', 0, 1, 50.25, 1, 'alex.png');
            """);
        _globalState.UseDatabase(db.DatabasePath);

        var person = await new PersonRepository().GetPersonbyIdAsync(8);

        Assert.Equal(8, person.PersonId);
        Assert.Equal("Alex", person.Name);
        Assert.Equal(PersonEnum.Individual, person.PersonRole);
        Assert.True(person.IsUser);
        Assert.Equal(50.25m, person.Funds);
    }

    [Fact]
    public async Task HouseRepository_reads_seeded_house()
    {
        using var db = new SqliteTestDatabase();
        await db.ExecuteScriptAsync("""
            CREATE TABLE House (
                HouseId INTEGER PRIMARY KEY,
                Name TEXT NOT NULL,
                Price NUMERIC NOT NULL,
                ForSale INTEGER NOT NULL,
                FrontImage TEXT NOT NULL,
                LivingRoomImage TEXT NOT NULL,
                GarageImage TEXT NOT NULL,
                OwnerName TEXT NOT NULL,
                GarageButtonLocX REAL NOT NULL,
                GarageButtonLocY REAL NOT NULL,
                GarageProductsLocX REAL NOT NULL,
                GarageProductsLocY REAL NOT NULL
            );
            INSERT INTO House (HouseId, Name, Price, ForSale, FrontImage, LivingRoomImage, GarageImage, OwnerName, GarageButtonLocX, GarageButtonLocY, GarageProductsLocX, GarageProductsLocY)
            VALUES (9, 'Blue House', 125000, 1, 'front.png', 'living.png', 'garage.png', 'Alex', 10, 20, 30, 40);
            """);
        _globalState.UseDatabase(db.DatabasePath);

        var house = await new HouseRepository().GetHouseByIdAsync(9);

        Assert.Equal(9, house.HouseId);
        Assert.Equal("Blue House", house.Name);
        Assert.Equal(125000m, house.Price);
        Assert.Equal(1, house.ForSale);
        Assert.Equal("Alex", house.OwnerName);
        Assert.Equal(10, house.GarageButtonLocX);
    }

    [Fact]
    public async Task PersonImageRepository_reads_seeded_person_image()
    {
        using var db = new SqliteTestDatabase();
        await db.ExecuteScriptAsync("""
            CREATE TABLE PersonImage (
                FKImageID INTEGER PRIMARY KEY,
                Name TEXT NOT NULL,
                PersonImageType INTEGER NOT NULL,
                FilePath TEXT NOT NULL,
                ImageType TEXT NOT NULL,
                FKPersonID INTEGER NOT NULL,
                PersonImageStatus INTEGER NOT NULL
            );
            INSERT INTO PersonImage (FKImageID, Name, PersonImageType, FilePath, ImageType, FKPersonID, PersonImageStatus)
            VALUES (1, 'AlexFront', 0, 'alex-front.png', 'png', 8, 0);
            """);
        _globalState.UseDatabase(db.DatabasePath);

        var image = await new PersonImageRepository().GetPersonImagebyNameAsync("AlexFront");

        Assert.Equal("AlexFront", image.Name);
        Assert.Equal("alex-front.png", image.FilePath);
        Assert.Equal(8, image.FKPersonId);
    }

    [Fact]
    public async Task PersonProductsOwnedRepository_reads_seeded_ownership()
    {
        using var db = new SqliteTestDatabase();
        await db.ExecuteScriptAsync("""
            CREATE TABLE PersonProductsOwned (
                PersonProductsOwnedId INTEGER PRIMARY KEY,
                PersonId INTEGER NOT NULL,
                ShopItemId INTEGER NOT NULL,
                Quantity INTEGER NOT NULL
            );
            INSERT INTO PersonProductsOwned (PersonProductsOwnedId, PersonId, ShopItemId, Quantity)
            VALUES (10, 8, 2, 3);
            """);
        _globalState.UseDatabase(db.DatabasePath);

        var owned = await new PersonProductsOwnedRepository().GetByIdAsync(10);

        Assert.Equal(10, owned.PersonProductsOwnedId);
        Assert.Equal(8, owned.PersonId);
        Assert.Equal(2, owned.ShopItemId);
        Assert.Equal(3, owned.Quantity);
    }

    [Fact]
    public async Task PoliceCarRepository_reads_seeded_police_car()
    {
        using var db = new SqliteTestDatabase();
        await db.ExecuteScriptAsync("""
            CREATE TABLE PoliceCar (
                PoliceCarId INTEGER PRIMARY KEY,
                CarName TEXT NOT NULL,
                ImageName TEXT NOT NULL,
                CarType TEXT NOT NULL
            );
            INSERT INTO PoliceCar (PoliceCarId, CarName, ImageName, CarType)
            VALUES (11, 'Patrol', 'patrol.png', 'Sedan');
            """);
        _globalState.UseDatabase(db.DatabasePath);

        var car = await new PoliceCarRepository().GetPoliceCarByIdAsync(11);

        Assert.Equal(11, car.PoliceCarId);
        Assert.Equal("Patrol", car.CarName);
        Assert.Equal("patrol.png", car.ImageName);
        Assert.Equal("Sedan", car.CarType);
    }

    [Fact]
    public async Task ShelfLocationRepository_reads_seeded_shelf_location()
    {
        using var db = new SqliteTestDatabase();
        await db.ExecuteScriptAsync("""
            CREATE TABLE ShelfLocation (
                ShelfLocationID INTEGER PRIMARY KEY,
                StoreType TEXT NOT NULL,
                Aisle TEXT NOT NULL,
                Shelf TEXT NOT NULL,
                PositionX REAL NOT NULL,
                PositionY REAL NOT NULL,
                PositionZ REAL NOT NULL,
                Width REAL NOT NULL,
                Height REAL NOT NULL,
                ShopItemId INTEGER NOT NULL
            );
            INSERT INTO ShelfLocation (ShelfLocationID, StoreType, Aisle, Shelf, PositionX, PositionY, PositionZ, Width, Height, ShopItemId)
            VALUES (12, 'Location', 'A', 'Top', 1.5, 2.5, 3.5, 10, 20, 2);
            """);
        _globalState.UseDatabase(db.DatabasePath);

        var shelf = await new ShelfLocationRepository().GetShelfLocationByIdAsync(12);

        Assert.Equal(12, shelf.ShelfLocationID);
        Assert.Equal("Location", shelf.StoreType);
        Assert.Equal("A", shelf.Aisle);
        Assert.Equal("Top", shelf.Shelf);
        Assert.Equal(1.5, shelf.PositionX);
        Assert.Equal(2, shelf.ShopItemId);
    }

    [Fact]
    public async Task LandscapeObjectRepository_reads_group_ids_from_seeded_objects()
    {
        using var db = new SqliteTestDatabase();
        await db.ExecuteScriptAsync("""
            CREATE TABLE LandScapeObject (
                LandScapeObjectID INTEGER PRIMARY KEY,
                Name TEXT NOT NULL,
                Description TEXT NOT NULL,
                Height REAL NOT NULL,
                Width REAL NOT NULL,
                xActual REAL NOT NULL,
                yActual REAL NOT NULL,
                ImageName TEXT NOT NULL,
                GroupId INTEGER NOT NULL,
                HomeObject INTEGER NOT NULL,
                NextFromHomeObject INTEGER NOT NULL,
                FeatureNote TEXT NOT NULL
            );
            INSERT INTO LandScapeObject (LandScapeObjectID, Name, Description, Height, Width, xActual, yActual, ImageName, GroupId, HomeObject, NextFromHomeObject, FeatureNote)
            VALUES (1, 'Tree', 'Oak', 10, 5, 1, 2, 'tree.png', 42, 0, 0, '');
            """);
        _globalState.UseDatabase(db.DatabasePath);

        var groupIds = new LandscapeObjectRepository().GetLandscapeObjectsGroupIds();

        Assert.Contains(42, groupIds);
    }

    [Fact]
    public async Task ImageRepository_reads_seeded_image_rows()
    {
        using var db = new SqliteTestDatabase();
        GlobalServices.ImageFolder = Path.GetTempPath();
        await db.ExecuteScriptAsync("""
            CREATE TABLE Image (
                ImageId INTEGER PRIMARY KEY,
                SequenceNumber INTEGER NOT NULL,
                Name TEXT NOT NULL,
                Filename TEXT NULL,
                FilePath TEXT NOT NULL,
                NamePart TEXT NOT NULL,
                NumberPart INTEGER NOT NULL,
                TrailingPart TEXT NOT NULL,
                ImageRole TEXT NOT NULL
            );
            INSERT INTO Image (ImageId, SequenceNumber, Name, Filename, FilePath, NamePart, NumberPart, TrailingPart, ImageRole)
            VALUES (1, 4, 'Machine', 'machine.png', 'C:\Images\machine.png', 'machine', 1, 'png', 'Object');
            """);
        _globalState.UseDatabase(db.DatabasePath);

        var images = await new ImageRepository().GetImagesAsync();

        var image = Assert.Single(images);
        Assert.Equal(4, image.SequenceNumber);
        Assert.Equal("machine", image.NamePart);
        Assert.Equal("C:\\Images\\machine.png", image.FilePath);
    }

    [Fact]
    public async Task LoggingRepository_reads_seeded_log_rows()
    {
        using var db = new SqliteTestDatabase();
        await db.ExecuteScriptAsync("""
            CREATE TABLE Logging (
                LoggingID INTEGER PRIMARY KEY,
                ClassName TEXT NOT NULL,
                MethodName TEXT NOT NULL,
                Message TEXT NOT NULL,
                RunTime NUMERIC NULL,
                ImageType NUMERIC NULL
            );
            INSERT INTO Logging (LoggingID, ClassName, MethodName, Message, RunTime, ImageType)
            VALUES (1, 'MachineLayoutDesigner', 'Save', 'Saved sequence', 1.25, 1.25);
            """);
        _globalState.UseDatabase(db.DatabasePath);

        var logs = await new LoggingRepository().GetLogAsync();

        var log = Assert.Single(logs);
        Assert.Equal(1, log.LoggingID);
        Assert.Equal("MachineLayoutDesigner", log.ClassName);
        Assert.Equal("Save", log.MethodName);
        Assert.Equal(1.25m, log.RunTime);
    }
}

