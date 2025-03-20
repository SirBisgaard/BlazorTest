using BlazorAppTest.Domain;
using BlazorAppTest.Domain.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BlazorAppTest.Controllers;

// I wanted a controller but I am going to embrace that u can use services directly in Razor pages & components.
// But this might be useful for other integration purposes down the road.
[AllowAnonymous]
[Route("api/[controller]")]
[ApiController]
public class LocationController (ILocationService locationService)
{
    [HttpGet]
    public IAsyncEnumerable<Location> GetLocations()
    {
        return locationService.GetLocations();
    }
}