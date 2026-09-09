using CatoriServices.Objects.database;
using CatoriServices.Objects.Entities;

namespace CatoriServices.Tests;

public sealed class LocationPartRouteRepositoryTests
{
    [Fact]
    public async Task LocationPartRouteRepository_documents_missing_live_route_tables_contract()
    {
        var repository = new LocationPartRouteRepository();

        Assert.Null(await repository.GetByIdAsync(1));
        Assert.Empty(await repository.GetByLayoutIdAsync(1));
        Assert.False(await repository.DeleteAsync(1));
        await Assert.ThrowsAsync<NotSupportedException>(() => repository.InsertAsync(new LocationPartRouteEntity()));
        await Assert.ThrowsAsync<NotSupportedException>(() => repository.InsertPointAsync(new LocationPartRoutePointEntity()));
    }
}
