using System.Data.SQLite;

namespace BlazorAppTest.DataAccess.Database;

public class SqLiteConnectionFactory(string connectionString) : ISqLiteConnectionFactory
{
    public async Task<SQLiteConnection> CreateConnection()
    {
        try
        {
            var connection = new SQLiteConnection(connectionString);
            await connection.OpenAsync();
            return connection;
        }
        catch (Exception e)
        {
            // Would be better to use custom exception type to better tracking and representing them higher up.
            throw new InvalidOperationException("Could not create a connection to the database", e);
        }
    }
}