using BlazorAppTest.DataAccess.Database;
using BlazorAppTest.Domain;
using BlazorAppTest.Domain.Interfaces;
using Dapper;

namespace BlazorAppTest.DataAccess;

public class LocationRepository : ILocationRepository
{
    private readonly ISqLiteConnectionFactory connectionFactory;

    public LocationRepository(ISqLiteConnectionFactory connectionFactory)
    {
        this.connectionFactory = connectionFactory;

        // Create the db file if it doesn't exist with some data
        EnsureDatabaseWithDataExists().Wait();
    }

    private async Task EnsureDatabaseWithDataExists()
    {
        const string createTableSql = @"
            CREATE TABLE IF NOT EXISTS Locations (
                Id INTEGER NOT NULL,
                Name TEXT NOT NULL,
                Address TEXT NOT NULL,
                Email TEXT NOT NULL,
                Phone TEXT NOT NULL
            );

            CREATE TABLE IF NOT EXISTS LocationCoverPhotos (
                Id INTEGER NOT NULL,
                Url TEXT NOT NULL,
                Title TEXT NOT NULL,
                LocationId INTEGER NOT NULL,
                FOREIGN KEY(LocationId) REFERENCES Locations(Id)
            );
            
            CREATE TABLE IF NOT EXISTS LocationActivity (
                Id INTEGER NOT NULL,
                Name TEXT NOT NULL,
                LocationId INTEGER NOT NULL,
                FOREIGN KEY(LocationId) REFERENCES Locations(Id)
            );
            
            CREATE TABLE IF NOT EXISTS LocationOpeningHours (
                Id INTEGER NOT NULL,
                Day TEXT NOT NULL,
                Open TEXT NOT NULL,
                Close TEXT NOT NULL,
                LocationActivityId INTEGER NOT NULL,
                FOREIGN KEY(LocationActivityId) REFERENCES LocationActivity(Id)
            );
        ";

        await using var connection = await connectionFactory.CreateConnection();
        await using var command = connection.CreateCommand();
        command.CommandText = createTableSql;
        await command.ExecuteNonQueryAsync();

        // Create data if the locations are not there...
        var locations = GetLocations();
        if (await locations.CountAsync() == 0)
        {
            const string createDataSql = @"
                INSERT INTO Locations (Id, Name, Address, Email, Phone) VALUES (1, 'Aalborg', 'Some address 1, 9000 Aalborg', 'aalborg@blazor.test', '+45 12 34 56 78');
                INSERT INTO Locations (Id, Name, Address, Email, Phone) VALUES (2, 'Svendborg', 'Some address 32, 5700 Svendborg', 'svendborg@blazor.test', '+45 12 34 56 78');
                
                INSERT INTO LocationCoverPhotos (Id, Url, Title, LocationId) VALUES (1, '1.png', 'Hall', 1);
                INSERT INTO LocationCoverPhotos (Id, Url, Title, LocationId) VALUES (2, '2.png', 'Building', 1);
                INSERT INTO LocationCoverPhotos (Id, Url, Title, LocationId) VALUES (3, '3.png', 'Buffet', 1);
                INSERT INTO LocationCoverPhotos (Id, Url, Title, LocationId) VALUES (4, '4.png', 'Conference Hall', 2);
                INSERT INTO LocationCoverPhotos (Id, Url, Title, LocationId) VALUES (5, '5.png', 'Meeting Room', 2);
                
                INSERT INTO LocationActivity (Id, Name, LocationId) VALUES (1, 'Conference', 1);
                INSERT INTO LocationActivity (Id, Name, LocationId) VALUES (2, 'Meeting', 1);
                INSERT INTO LocationActivity (Id, Name, LocationId) VALUES (3, 'Buffet', 1);
                INSERT INTO LocationActivity (Id, Name, LocationId) VALUES (4, 'Conference', 2);
                INSERT INTO LocationActivity (Id, Name, LocationId) VALUES (5, 'Meeting', 2);
                INSERT INTO LocationActivity (Id, Name, LocationId) VALUES (6, 'Ice Melting', 2);
                
                INSERT INTO LocationOpeningHours (Id, Day, Open, Close, LocationActivityId) VALUES (1, 'Monday-Friday', '08:00', '16:00', 1);
                INSERT INTO LocationOpeningHours (Id, Day, Open, Close, LocationActivityId) VALUES (2, 'Weekend', '00:00', '00:00', 1);
                INSERT INTO LocationOpeningHours (Id, Day, Open, Close, LocationActivityId) VALUES (3, 'Holidays', '00:00', '00:00', 1);
                INSERT INTO LocationOpeningHours (Id, Day, Open, Close, LocationActivityId) VALUES (4, 'Monday-Friday', '08:00', '16:00', 2);
                INSERT INTO LocationOpeningHours (Id, Day, Open, Close, LocationActivityId) VALUES (5, 'Weekend', '08:00', '16:00', 2);
                INSERT INTO LocationOpeningHours (Id, Day, Open, Close, LocationActivityId) VALUES (6, 'Holidays', '00:00', '00:00', 2);
                INSERT INTO LocationOpeningHours (Id, Day, Open, Close, LocationActivityId) VALUES (7, 'Monday-Friday', '13:00', '16:00', 3);
                INSERT INTO LocationOpeningHours (Id, Day, Open, Close, LocationActivityId) VALUES (8, 'Weekend', '01:00', '23:00', 3);
                INSERT INTO LocationOpeningHours (Id, Day, Open, Close, LocationActivityId) VALUES (9, 'Holidays', '00:00', '00:00', 3);
                INSERT INTO LocationOpeningHours (Id, Day, Open, Close, LocationActivityId) VALUES (10, 'Monday-Friday', '08:00', '23:50', 4);
                INSERT INTO LocationOpeningHours (Id, Day, Open, Close, LocationActivityId) VALUES (11, 'Weekend', '08:00', '16:00', 4);
                INSERT INTO LocationOpeningHours (Id, Day, Open, Close, LocationActivityId) VALUES (12, 'Holidays', '08:00', '16:00', 4);
                INSERT INTO LocationOpeningHours (Id, Day, Open, Close, LocationActivityId) VALUES (13, 'Monday-Friday', '01:00', '14:00', 5);
                INSERT INTO LocationOpeningHours (Id, Day, Open, Close, LocationActivityId) VALUES (14, 'Weekend', '08:00', '16:00', 5);
                INSERT INTO LocationOpeningHours (Id, Day, Open, Close, LocationActivityId) VALUES (15, 'Holidays', '00:00', '00:00', 5);
                INSERT INTO LocationOpeningHours (Id, Day, Open, Close, LocationActivityId) VALUES (16, 'Monday-Friday', '00:00', '00:00', 6);
                INSERT INTO LocationOpeningHours (Id, Day, Open, Close, LocationActivityId) VALUES (17, 'Weekend', '00:00', '00:00', 6);
                INSERT INTO LocationOpeningHours (Id, Day, Open, Close, LocationActivityId) VALUES (18, 'Holidays', '00:00', '00:00', 6);
            ";

            command.CommandText = createDataSql;
            await command.ExecuteNonQueryAsync();
        }
    }

    public async IAsyncEnumerable<Location> GetLocations()
    {
        await using var connection = await connectionFactory.CreateConnection();
        await using var reader = await connection.ExecuteReaderAsync("SELECT * FROM Locations");
        var rowParser = reader.GetRowParser<Location>();

        while (await reader.ReadAsync())
        {
            var location = rowParser(reader);

            // Worst mapping but quick and dirty instead of joining it in the query and mapping it with Dapper. 
            location.CoverPhotos = await connection.QueryAsync<LocationCoverPhotos>("SELECT * FROM LocationCoverPhotos WHERE LocationId = @LocationId", new { LocationId = location.Id });
            location.LocationActivities = await connection.QueryAsync<LocationActivity>("SELECT * FROM LocationActivity WHERE LocationId = @LocationId", new { LocationId = location.Id });
            
            foreach (var activity in location.LocationActivities)
            {
                activity.OpeningHours = await connection.QueryAsync<LocationOpeningHours>("SELECT * FROM LocationOpeningHours WHERE LocationActivityId = @LocationActivityId", new { LocationActivityId = activity.Id });
            }

            yield return location;
        }
    }
}