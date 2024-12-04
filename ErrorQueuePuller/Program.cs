class Program
{
    static async Task Main(string[] args)
    {
        Console.Title = "MyEndpointHost";

        // Configure and start the endpoint
        var endpointConfiguration = EndpointConfig.ConfigureEndpoint();
        var endpointInstance = await Endpoint.Start(endpointConfiguration)
            .ConfigureAwait(false);
        await endpointInstance.SendLocal(new MyMessage { Id = Guid.NewGuid(), Name = "Test1" });
        Console.WriteLine("Endpoint started. Press Enter to exit.");
        Console.ReadLine();

        // Stop the endpoint
        await endpointInstance.Stop()
            .ConfigureAwait(false);
    }
}
