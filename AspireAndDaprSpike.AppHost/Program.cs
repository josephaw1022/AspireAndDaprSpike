using Aspire.Hosting.Dapr;

var builder = DistributedApplication.CreateBuilder(args);

var postgresDatabaseServer = builder.AddPostgres("PostgresDB", 5432, "DaprPostgresPassword")
        .WithPgAdmin()
        .WithBindMount("./postgres-config/config-store", "/docker-entrypoint-initdb.d");


var configurationDatabase = postgresDatabaseServer.AddDatabase("ConfigurationDB");


var configurationStore = builder.AddDaprComponent("pg-config", "configuration", new()
{
    LocalPath = "./dapr/ConfigStore.yaml",
});


var microservice1 = builder.AddProject<Projects.microservice_01>("microservice01")
    .WithDaprSidecar(new DaprSidecarOptions
    {
        AppHealthThreshold = 15,
    })
    .WithReference(configurationStore);





builder.Build().Run();
