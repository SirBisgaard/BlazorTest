using BlazorAppTest.Domain;

namespace BlazorAppTest.Business.ComponentModels;

public class LocationViewModel
{
    public LocationActivity? Activity { get; set; }
    public event Action? OnChange; 
    
    public void ChangeActivity(LocationActivity? activity)
    {
        Activity = activity;
        OnChange?.Invoke();
    }
}