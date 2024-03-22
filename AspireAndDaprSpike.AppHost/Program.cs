using AspireAndDaprSpike.AppHost;

var builder = DistributedApplication.CreateBuilder(args);


// Dapr components

// The postgres configuration store
var configurationStore1 = builder.AddDaprComponent("config-store-1", "configuration", new()
{
    LocalPath = "./dapr/ConfigStore1.yaml",
});
 
// The redis configuration store
var configurationStore2 = builder.AddDaprComponent("config-store-2", "configuration", new()
{
    LocalPath = "./dapr/ConfigStore2.yaml",
});

// The mongo state store
var stateStore1 = builder.AddDaprComponent("state-store-1", "state", new()
{
    LocalPath = "./dapr/StateStore1.yaml",
});

// The postgres state store
var stateStore2 = builder.AddDaprComponent("state-store-2", "state", new()
{
    LocalPath = "./dapr/StateStore2.yaml",
});

// Executable To Host Dapr Dashboard
var dashboard = builder.AddExecutable("dapr-dashboard", "dapr", ".", "dashboard")
    .WithHttpEndpoint(containerPort: 8080, hostPort: 8080, name: "dashboard-http", isProxied: false)
    .ExcludeFromManifest();


// Microservices
builder.AddProject<Projects.microservice_01>("microservice01")
    .WithDaprSidecar(DaprSidecarOptionsHelper.CreateDaprSidecarOptions("microservice01-dapr"))
    .WithReference(configurationStore1)
    .WithReference(stateStore1);

builder.AddProject<Projects.microservice_02>("microservice02")
    .WithDaprSidecar(DaprSidecarOptionsHelper.CreateDaprSidecarOptions("microservice02-dapr"))
    .WithReference(configurationStore2)
    .WithReference(stateStore2);

// Start the applications
await builder.Build().RunAsync();
