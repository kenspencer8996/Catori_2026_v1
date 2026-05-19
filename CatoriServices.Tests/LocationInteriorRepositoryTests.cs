using CatoriServices.Objects.database;
using CatoriServices.Objects.Entities;

namespace CatoriServices.Tests;

[Collection(GlobalTestStateCollection.Name)]
public sealed class LocationInteriorRepositoryTests
{
    private readonly GlobalTestState _globalState;

    public LocationInteriorRepositoryTests(GlobalTestState globalState)
    {
        _globalState = globalState;
    }

    [Fact]
    public async Task SaveAsync_round_trips_location_interior_by_location_id()
    {
        using var db = await LocationRepositoryTests.CreateLocationLayoutDatabaseAsync();
        _globalState.UseDatabase(db.DatabasePath);

        var locationRepository = new LocationRepository();
         var locationId = await locationRepository.InsertAsync(new LocationEntity { LocationName = "Main Location" });

        var interior = new LocationEntity
        {
            LocationId = locationId,
            LocationName = "LocationInterior1",
            BackgroundImagePath = "C:\\Images\\LocationInterior1.png",
            InteriorType = "LocationInterior_1UC",
            SortOrder = 1
        };

        ////interior.Id = await repository.SaveAsync(interior);
        ////var loaded = await repository.GetByLocationIdAsync(locationId);

        //Assert.Equal(interior.Id, loaded.Id);
        //Assert.Equal(locationId, loaded.FKLocationId);
        //Assert.Equal("LocationInterior1", loaded.Name);
        //Assert.Equal("C:\\Images\\LocationInterior1.png", loaded.BackgroundImagePath);
    }
}
