using System.Data.SQLite;
using BlazorAppTest.DataAccess.Database;
using BlazorAppTest.Domain;
using BlazorAppTest.Domain.Interfaces;

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
                Id INTEGER PRIMARY KEY,
                Name TEXT NOT NULL
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
                INSERT INTO Locations (Name) VALUES ('Copenhagen');
                INSERT INTO Locations (Name) VALUES ('Odense');
                INSERT INTO Locations (Name) VALUES ('Svendborg');
            ";
            
            command.CommandText = createDataSql;
            await command.ExecuteNonQueryAsync();
        }
    }

    public async IAsyncEnumerable<Location> GetLocations()
    {
        const string sql = @"
            SELECT * FROM Locations;
        ";

        await using var connection = await connectionFactory.CreateConnection();
        await using var command = connection.CreateCommand();
        command.CommandText = sql;

        await using var reader = await command.ExecuteReaderAsync();
        if (!reader.HasRows)
        {
            yield break;
        }
        
        while (await reader.ReadAsync())
        {
            yield return new Location
            {
                Id = reader.GetInt32(0),
                Name = reader.GetString(1)
            };
        }
    }
}