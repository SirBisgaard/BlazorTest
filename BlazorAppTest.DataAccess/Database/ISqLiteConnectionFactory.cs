using System.Data.SQLite;

namespace BlazorAppTest.DataAccess.Database;

public interface ISqLiteConnectionFactory
{
    Task<SQLiteConnection> CreateConnection();
}