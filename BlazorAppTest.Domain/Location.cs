namespace BlazorAppTest.Domain;

public class Location
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Address { get; set; }
    public string Email { get; set; }
    public string Phone { get; set; }
    
    public IEnumerable<LocationCoverPhotos> CoverPhotos { get; set; }
    public IEnumerable<LocationActivity> LocationActivities { get; set; }
}

public class LocationActivity
{
    public int Id { get; set; }
    public string Name { get; set; }
    public IEnumerable<LocationOpeningHours> OpeningHours { get; set; }
}

public class LocationOpeningHours
{
    public int Id { get; set; }
    public string Day { get; set; }
    // Well we cannot store dates or times in SQLite, so we store them as strings
    // Dapper is not easily able to convert strings to TimeSpan, so we will continue to use strings...
    public string Open { get; set; }
    public string Close { get; set; }
}

public class LocationCoverPhotos
{
    public int Id { get; set; }
    public string Url { get; set; }
    public string Title { get; set; }
}
