namespace BlazorAppTest.Domain.Interfaces;

public interface ILocationService
{
    IAsyncEnumerable<Location> GetLocations();
}