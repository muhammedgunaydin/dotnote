using dot_note.DbContext;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();

builder.Services.AddSingleton<IMongoClient>(ServiceProvider =>
{
    var configuration = ServiceProvider.GetRequiredService<IConfiguration>();
    var connectionString = configuration.GetConnectionString("MongoDBConnection");

    return new MongoClient(connectionString);
});

// MongoDbContext servisini ekleyin
builder.Services.AddScoped<MongoDbContext>(ServiceProvider =>
{
    var mongoClient = ServiceProvider.GetRequiredService<IMongoClient>();

    var databaseName = "dotnote";

    return new MongoDbContext(mongoClient, databaseName);
});

builder.Services.AddSession();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.UseSession();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=User}/{action=Login}/{id?}");

try
{
    var mongoClient = app.Services.GetRequiredService<IMongoClient>();
    mongoClient.StartSession();
    Console.WriteLine("Successfully connected to MongoDB");
}
catch (Exception ex)
{
    Console.WriteLine($"Mongo conn error: {ex.Message}");
}

app.Run();