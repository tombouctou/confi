using Astor.Logging;
using Scalar.AspNetCore;
using Fluenv;
using Nist.Logs;
using MongoDB.Driver;
using Persic;
using Confi;
using Nist;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);
builder.Logging.ClearProviders();
builder.Logging.AddMiniJsonConsole();
builder.Logging.AddSimpleConsole(c => c.SingleLine = true);

builder.Configuration.AddFluentEnvironmentVariables();

builder.Services.AddProblemDetails();
builder.Services.AddOpenApi();
builder.Services.AddMongo(
    _ => new MongoClient(builder.Configuration.GetRequiredValue("ConnectionStrings:Mongo")), 
    "confi-manager"
)
.AddConfiManagerCollections();

builder.Services.AddEndpointsApiExplorer(); // Add this line
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Confi.Manager API",
        Version = "v1",
        Description = "API for Confi.Manager"
    });
});

var app = builder.Build();

var env = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
if (app.Environment.IsDevelopment() || env == "Local")
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Confi.Manager API V1");
    });
}

app.MapOpenApi();
app.MapScalarApiReference(s => s.WithTheme(ScalarTheme.DeepSpace));

app.UseHttpIOLogging(l => l.Message = HttpIOMessagesRegistry.DefaultWithJsonBodies);
app.UseProblemForExceptions(ex => 
    ex.ToConfiManagerError() ?? Errors.Unknown
);

app.MapGet($"/{Uris.About}", async (IHostEnvironment env, IMongoDatabase mongoDatabase) => new About(
    Description: "Confi.Manager",
    Version: typeof(Program).Assembly!.GetName().Version!.ToString(),
    Environment: env.EnvironmentName,
    Dependencies: new() {
        [ "mongo" ] = await mongoDatabase.Ping().ToJsonDocument()
    }
));

var prefix = app.Configuration["Prefix"];
_ = string.IsNullOrWhiteSpace(prefix)
    ? app.MapConfiManager()
    : app.MapGroup(prefix).MapConfiManager();

await app.Services.ApplyMongoConfiguration();

app.Lifetime.ApplicationStarted.Register(() =>
{
    var addresses = app.Urls;
    foreach (var address in addresses)
    {
        app.Logger.LogInformation("Application listening on: {Address}", address);
    }
});

app.Run();

public partial class Program;