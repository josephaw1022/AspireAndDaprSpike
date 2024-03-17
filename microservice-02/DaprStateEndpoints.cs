using Dapr.Client;

namespace microservice_02;

public static class DaprStateExtensions
{
    private const string stateStore = "state-store-2";

    public static IEndpointRouteBuilder MapDaprStateCRUDEndpoints(this IEndpointRouteBuilder endpoints, string basePattern)
    {
        // Get state
        endpoints.MapGet($"{basePattern}/{{id}}", async (DaprClient daprClient, string id) =>
        {
            var state = await daprClient.GetStateAsync<RandomState2>(stateStore, id);
            return state != null ? Results.Ok(state) : Results.NotFound();
        })
        .WithName("GetState")
        .WithOpenApi();



        // Create or update state
        endpoints.MapPost(basePattern, async (DaprClient daprClient, RandomState2 randomState) =>
        {
            await daprClient.SaveStateAsync(stateStore, randomState.Id, randomState);
            return Results.Ok();
        })
        .WithName("SaveState")
        .WithOpenApi();

        // Delete state
        endpoints.MapDelete($"{basePattern}/{{id}}", async (DaprClient daprClient, string id) =>
        {
            await daprClient.DeleteStateAsync(stateStore, id);
            return Results.Ok();
        })
        .WithName("DeleteState")
        .WithOpenApi();

        return endpoints;
    }
}

public class RandomState2
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string Description { get; set; } = "Random Description";
    public double Price { get; set; } = 99.99;
}
