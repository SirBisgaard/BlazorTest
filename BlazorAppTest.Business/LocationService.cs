using BlazorAppTest.Domain;
using BlazorAppTest.Domain.Interfaces;

namespace BlazorAppTest.Business;

public class LocationService (ILocationRepository locationRepository) : ILocationService
{
    public async IAsyncEnumerable<Location> GetLocations()
    {
        await foreach (var location in locationRepository.GetLocations())
        {
            yield return location;
        }
    }
}