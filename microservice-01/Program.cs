using System.Text.Json;
using Dapr.Client;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


builder.Services.AddDaprClient();

// Bind configuration values from Dapr
var daprClient = new DaprClientBuilder().Build();
var allConfigValues = await daprClient.GetConfiguration("pg-config", new List<string>());
var configJson = JsonSerializer.Serialize(allConfigValues);
var configRoot = JsonSerializer.Deserialize<Dictionary<string, Dictionary<string, ConfigItem>>>(configJson);

var configDictionary = new Dictionary<string, string>();
foreach (var item in configRoot["items"])
{
    configDictionary[$"AppConfig:Items:{item.Key}:Value"] = item.Value.Value;
    configDictionary[$"AppConfig:Items:{item.Key}:Version"] = item.Value.Version;
    configDictionary[$"AppConfig:Items:{item.Key}:Metadata:Author"] = item.Value.Metadata.Author;
    configDictionary[$"AppConfig:Items:{item.Key}:Metadata:CreatedAt"] = item.Value.Metadata.CreatedAt;
}

builder.Host.ConfigureAppConfiguration((hostingContext, config) =>
{
    config.AddInMemoryCollection(configDictionary);
});
// Bind configuration values from Dapr
// var daprClient = new DaprClientBuilder().Build();
// var configValues = await daprClient.GetConfiguration("pg-config", new List<string>() { "theme", "language", "notifications", "timezone", "currency" });

// var configDictionary = new Dictionary<string, string>();
// await foreach (var item in configValues.Items.Values.())
// {
//     configDictionary[$"AppConfig:Items:{item.Key}:Value"] = item.Value.Value;
//     configDictionary[$"AppConfig:Items:{item.Key}:Version"] = item.Value.Version;
//     configDictionary[$"AppConfig:Items:{item.Key}:Metadata:Author"] = item.Value.Metadata.Author;
//     configDictionary[$"AppConfig:Items:{item.Key}:Metadata:CreatedAt"] = item.Value.Metadata.CreatedAt;
// }

// var configBuilder = new ConfigurationBuilder()
//     .AddInMemoryCollection(configDictionary)
//     .AddConfiguration(builder.Configuration);

// builder.Configuration = configBuilder.Build();



var app = builder.Build();

app.MapDefaultEndpoints();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}



app.MapGet("/", async (DaprClient daprClient) =>
{
    var allConfigValues = await daprClient.GetConfiguration("pg-config", new List<string>());

    return Results.Ok(allConfigValues);
})
.WithName("Dapr Config Store Values")
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
