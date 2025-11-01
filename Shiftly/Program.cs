// Program.cs - API versie met Swagger
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.AspNetCore.Hosting.Server.Features;
using DAL;
using DAL.Repositories;
using BLL.Services;
using Shiftly.Data;

var builder = WebApplication.CreateBuilder(args);

// Add API Controllers
builder.Services.AddControllers();

// Add Swagger/OpenAPI
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Title = "Shiftly API",
        Version = "v1",
        Description = "Voetbalplanning API",
        Contact = new Microsoft.OpenApi.Models.OpenApiContact
        {
            Name = "Shiftly Team"
        }
    });

    // XML comments voor betere documentatie
    var xmlFile = $"{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    options.IncludeXmlComments(xmlPath);
});

// Database
builder.Services.AddDbContext<ShiftlyContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("ShiftlyConnection")));

// Repositories (DAL)
builder.Services.AddScoped<AdminRepository>();
builder.Services.AddScoped<SpelerRepository>();
builder.Services.AddScoped<WedstrijdRepository>();
builder.Services.AddScoped<WedstrijdSpelerRepository>();

// Services (BLL)
builder.Services.AddScoped<AdminService>();
builder.Services.AddScoped<SpelerService>();
builder.Services.AddScoped<WedstrijdService>();
builder.Services.AddScoped<AfwezigheidService>();

// CORS (optioneel, als je frontend vanaf andere origin gebruikt)
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

var app = builder.Build();

// Database initialisatie en seeding
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<ShiftlyContext>();

    // Voor ontwikkeling: verwijder oude database en maak nieuwe aan met test data
    // WAARSCHUWING: EnsureDeleted verwijdert alle data!
    // Voor productie: gebruik migrations in plaats van EnsureCreated
    context.Database.EnsureDeleted();

    DbInitializer.Initialize(context);
}

// Configure middleware
// Swagger altijd inschakelen (niet alleen in Development)
app.UseSwagger();
app.UseSwaggerUI(options =>
{
    options.SwaggerEndpoint("/swagger/v1/swagger.json", "Shiftly API v1");
    options.RoutePrefix = string.Empty; // Swagger UI op root URL
});

app.UseHttpsRedirection();

// CORS (als je het hebt ingeschakeld)
app.UseCors("AllowAll");

app.UseAuthorization();

app.MapControllers();

// Start app en toon URLs
app.Lifetime.ApplicationStarted.Register(() =>
{
    var addresses = app.Services.GetRequiredService<IServer>().Features.Get<IServerAddressesFeature>();
    Console.WriteLine("\n🚀 Shiftly API gestart!");
    if (addresses != null)
    {
        foreach (var address in addresses.Addresses)
        {
            Console.WriteLine($"📖 Swagger UI beschikbaar op: {address}");
        }
    }
    Console.WriteLine();
});

app.Run();