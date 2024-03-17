using Dapr.Client;
using microservice_02;

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
    var configValues = await daprClient.GetConfiguration("config-store-2", new List<string>() {});

    return Results.Ok(configValues);
})
.WithName("Configure Store 2 Items")
.WithOpenApi();


app.MapDaprStateCRUDEndpoints("state-2");

app.Run();