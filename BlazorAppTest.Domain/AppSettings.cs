namespace BlazorAppTest.Domain;

public class AppSettings
{
    public ConnectionStrings ConnectionStrings { get; set; }
}

public class ConnectionStrings
{
    public string SqLiteConnection { get; set; } 
}