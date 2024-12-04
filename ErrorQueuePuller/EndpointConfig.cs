using Microsoft.Data.SqlClient;
using NServiceBus;

public class EndpointConfig
{
    public static EndpointConfiguration ConfigureEndpoint()
    {
        var endpointConfiguration = new EndpointConfiguration("ErrorQueuePuller");

        // Transport Configuration (RabbitMQ)
        var transport = endpointConfiguration.UseTransport<RabbitMQTransport>();
        transport.ConnectionString("host=localhost;username=guest;password=guest");
        transport.UseConventionalRoutingTopology(QueueType.Quorum);

        //var routing = transport.Routing();
        //routing.RouteToEndpoint(typeof(MyMessage), "ErrorQueuePuller");

        // Persistence Configuration
        var persistence = endpointConfiguration.UsePersistence<SqlPersistence>();
        persistence.SqlDialect<SqlDialect.MsSqlServer>().Schema("NServiceBus");
        persistence.ConnectionBuilder(
                connectionBuilder: () =>
                {
                    return new SqlConnection("Server=.\\localhost;Database=Practice;Integrated Security=True;");
                }
        );
        // Error queue
        endpointConfiguration.SendFailedMessagesTo("error");

        // Audit queue
        endpointConfiguration.AuditProcessedMessagesTo("audit");

        // Enable Installers (auto-create queues, etc.)
        endpointConfiguration.EnableInstallers();

        // Concurrency and throughput
        endpointConfiguration.LimitMessageProcessingConcurrencyTo(1);

        endpointConfiguration.UseSerialization<NewtonsoftJsonSerializer>();

        return endpointConfiguration;
    }
}
