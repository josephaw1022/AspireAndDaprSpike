using System.Text.Json;
using Dapr.Client;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


builder.Services.AddDaprClient();

// Create Dapr client
var daprClient = new DaprClientBuilder().Build();

// Fetch all configuration values from Dapr
var allConfigValues = await daprClient.GetConfiguration("pg-config", new List<string>());

// Serialize the response to JSON and then deserialize it into the desired data structure
var configJson = JsonSerializer.Serialize(allConfigValues);
var configRoot = JsonSerializer.Deserialize<Dictionary<string, Dictionary<string, ConfigItem>>>(configJson);

// Convert the configuration values into a dictionary that can be used with AddInMemoryCollection
var configDictionary = new Dictionary<string, string?>();
if (configRoot != null && configRoot.ContainsKey("items"))
{
    foreach (var item in configRoot["items"])
    {
        configDictionary[$"AppConfig:Items:{item.Key}:Value"] = item.Value.Value;
        configDictionary[$"AppConfig:Items:{item.Key}:Version"] = item.Value.Version;
        if (item.Value.Metadata != null)
        {
            configDictionary[$"AppConfig:Items:{item.Key}:Metadata:Author"] = item.Value.Metadata.Author;
            configDictionary[$"AppConfig:Items:{item.Key}:Metadata:CreatedAt"] = item.Value.Metadata.CreatedAt;
        }
    }
}

// Add the configuration dictionary to the application's configuration
builder.Host.ConfigureAppConfiguration((hostingContext, config) =>
{
    config.AddInMemoryCollection(configDictionary);
});





var app = builder.Build();

app.MapDefaultEndpoints();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}


app.MapGet("/", (IConfiguration configuration) =>
{
    var configValue = configuration["AppConfig:Items:theme:Value"];
    var configVersion = configuration["AppConfig:Items:theme:Version"];
    var configAuthor = configuration["AppConfig:Items:theme:Metadata:Author"];
    var configCreatedAt = configuration["AppConfig:Items:theme:Metadata:CreatedAt"];

    return Results.Ok(new { value = $"Configuration Value: {configValue}, Version: {configVersion}, Author: {configAuthor}, Created At: {configCreatedAt}" });
})
.WithName("All Configuration Items")
.WithOpenApi();


app.Run();




public class ConfigItem
{
    public string Value { get; set; }
    public string Version { get; set; }
    public Metadata Metadata { get; set; }
}

public class Metadata
{
    public string Author { get; set; }
    public string CreatedAt { get; set; }
}
