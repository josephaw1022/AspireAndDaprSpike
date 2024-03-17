using Dapr.Client;
using microservice_01;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


builder.Services.AddDaprClient();


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
    var configItems = await daprClient.GetConfiguration("config-store-1", new List<string> {});
    return Results.Ok(configItems);
})
.WithName("All Configuration Items")
.WithOpenApi();

app.MapDaprStateCRUDEndpoints("state");


app.Run();