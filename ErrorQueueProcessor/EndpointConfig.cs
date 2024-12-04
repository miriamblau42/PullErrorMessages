using NServiceBus;
using System.Net;
using System;
using Microsoft.Extensions.Logging;
using System.Data.SqlClient;
using System.Configuration;

namespace ErrorQueueProcessor
{
    public class EndpointConfig : IConfigureThisEndpoint
    {
        static public readonly string endpointName = "ErrorQueueProcessor";

        static public SendOptions endpointDestinationOptions = new SendOptions();
        public void Customize(EndpointConfiguration endpointConfiguration)
        {
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Ssl3 | SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;
            endpointConfiguration.UseTransport<RabbitMQTransport();
            UseSqlPersistence(endpointConfiguration);

            endpointConfiguration.RegisterComponents(
              registration: configureComponents =>
              {
                  configureComponents.ConfigureComponent
                  <Program>
                  (DependencyLifecycle.SingleInstance);

              }
          );

            endpointDestinationOptions.SetDestination(endpointName);
        }

        void UseSqlPersistence(EndpointConfiguration endpointConfiguration)
        {
            var persistence = endpointConfiguration.UsePersistence<SqlPersistence>();
            persistence.SqlDialect<SqlDialect.MsSqlServer>().Schema("NServiceBus");
            persistence.ConnectionBuilder(
                    connectionBuilder: () =>
                    {
                        return new SqlConnection(ConfigurationManager.ConnectionStrings["ArixDb"].ConnectionString);
                    }
            );
            persistence.SubscriptionSettings().CacheFor(TimeSpan.FromMinutes(1));
        }
    }
}
