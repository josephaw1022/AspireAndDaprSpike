using Dapr.Client;

namespace microservice_01;

public static class DaprStateExtensions
{

    private const string stateStore = "state-store-1";

    public static IEndpointRouteBuilder MapDaprStateCRUDEndpoints(this IEndpointRouteBuilder endpoints, string basePattern)
    {
        // Get state
        endpoints.MapGet($"{basePattern}/{{id}}", async (DaprClient daprClient, string id) =>
        {
            var state = await daprClient.GetStateAsync<RandomState1>(stateStore, id);
            return state != null ? Results.Ok(state) : Results.NotFound();
        })
        .WithName("GetState")
        .WithOpenApi();

        // Create or update state
        endpoints.MapPost(basePattern, async (DaprClient daprClient, RandomState1 randomState) =>
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



public class RandomState1
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string Name { get; set; } = "Random Name";
    public int Age { get; set; } = 42;
}