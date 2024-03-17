using Aspire.Hosting.Dapr;

var builder = DistributedApplication.CreateBuilder(args);

// Dapr components
var configurationStore1 = builder.AddDaprComponent("config-store-1", "configuration", new()
{
    LocalPath = "./dapr/ConfigStore1.yaml",
});

var configurationStore2 = builder.AddDaprComponent("config-store-2", "configuration", new()
{
    LocalPath = "./dapr/ConfigStore2.yaml",
});

var dashboard = builder.AddExecutable("dapr-dashboard", "dapr", ".", "dashboard" )
    .WithHttpEndpoint(containerPort: 8080, hostPort: 8080, name:"dashboard-http", isProxied: false);


// Dapr sidecar options
static DaprSidecarOptions CreateDaprSidecarOptions(string appId)
{
    return new()
    {
        AppHealthThreshold = 20,
        AppId = appId,
    };
}

// Microservices

builder.AddProject<Projects.microservice_01>("microservice01")
    .WithDaprSidecar(CreateDaprSidecarOptions("microservice01-dapr"))
    .WithReference(configurationStore1);

builder.AddProject<Projects.microservice_02>("microservice02")
    .WithDaprSidecar(CreateDaprSidecarOptions("microservice02-dapr"))
    .WithReference(configurationStore2);

// Start the applications
await builder.Build().RunAsync();