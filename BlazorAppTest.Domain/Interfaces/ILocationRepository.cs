namespace BlazorAppTest.Domain.Interfaces;

public interface ILocationRepository
{
    public IAsyncEnumerable<Location> GetLocations();
}