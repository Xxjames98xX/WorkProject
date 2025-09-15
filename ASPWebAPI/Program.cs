using ASPWebAPI.Database;
using Microsoft.EntityFrameworkCore;

using Serilog;
using Serilog.Events;
using Serilog.Configuration;
using Serilog.Extensions.Logging; // For integrating Serilog with Microsoft.Extensions.Logging
using Serilog.Settings.Configuration;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
// For MS SQL Server Database
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<AccountsDBContext>(options =>
    options.UseSqlServer(connectionString));
// For Logging based on Configuration in appsettings.json, LoggingProvider can be "Serilog" or "None"
var loggingProvider = builder.Configuration["LoggingProvider"];
if (loggingProvider == "Serilog")
{
    Log.Logger = new LoggerConfiguration()
        .ReadFrom.Configuration(builder.Configuration) // Read settings from appsettings.json, also possible to just write the configs in code
        .CreateLogger();
    builder.Host.UseSerilog();
}

// For dockerization, configure Kestrel to listen on port 5000 for HTTP, without it unable to run in docker
builder.WebHost.ConfigureKestrel(options =>
{
    options.ListenAnyIP(5000); // HTTP only
    // Skip HTTPS for dev/prototype
});


// Build the app
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    // Configurations for Swagger UI, so dont have to go to type in /swagger/index.html
    // c => {code here} is a lambda expression, like a temporary function without having to define it first i.e void c() {}
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1");
        c.RoutePrefix = string.Empty; // this moves swagger to the root "/"
    });

    //Using HTTP for simplicity (Prototyping) and not HTTPS
    app.Urls.Add("http://localhost:5000");
    
}

if (app.Environment.IsProduction())
{
    app.UseHttpsRedirection();
}

app.MapControllers(); // Maps attribute-routed API controllers, enabling them to handle requests
app.Run();
