using BlazorAppTest.Business;
using BlazorAppTest.Business.ComponentModels;
using BlazorAppTest.Components;
using BlazorAppTest.DataAccess;
using BlazorAppTest.DataAccess.Database;
using BlazorAppTest.Domain;
using BlazorAppTest.Domain.Interfaces;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

// Bind the AppSettings configuration
var appSettings = new AppSettings();
builder.Configuration.Bind(appSettings);

// Add external dependencies
builder.Services.AddSingleton<ISqLiteConnectionFactory>(new SqLiteConnectionFactory(appSettings.ConnectionStrings.SqLiteConnection));

// Add local services
builder.Services.AddScoped<ILocationService, LocationService>();
builder.Services.AddScoped<ILocationRepository, LocationRepository>();

// Add local models - Like the good old days of MVVM
builder.Services.AddScoped<LocationViewModel>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();
app.UseAntiforgery();

app.MapControllers();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();