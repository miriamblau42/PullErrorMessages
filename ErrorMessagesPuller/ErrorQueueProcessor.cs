using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;

public class ErrorQueueProcessor
{
    private const string RabbitMqHost = "localhost"; // RabbitMQ Host
    private const string ErrorQueueName = "error";   // NServiceBus Error Queue Name
    private const string TargetQueueName = "ErrorQueuePuller"; // Target Queue for Valid Messages

    public void ProcessErrorQueue()
    {
        // Setup RabbitMQ connection
        var factory = new ConnectionFactory()
        {
            HostName = RabbitMqHost,
            UserName = "guest", // Adjust for your environment
            Password = "guest"  // Adjust for your environment
        };

        using (var connection = factory.CreateConnection())
        using (var channel = connection.CreateModel())
        {
            // Declare the queues
            channel.QueueDeclare(ErrorQueueName, durable: true, exclusive: false, autoDelete: false, arguments: new Dictionary<string, object>
                {
                    { "x-queue-type", "quorum" }
                });
            channel.QueueDeclare(TargetQueueName, durable: true, exclusive: false, autoDelete: false, new Dictionary<string, object>
                {
                    { "x-queue-type", "quorum" }
                });

            var consumer = new EventingBasicConsumer(channel);

            consumer.Received += (model, ea) =>
            {
                var body = ea.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);

                // Extract headers to check message type
                if (ea.BasicProperties.Headers != null &&
                    ea.BasicProperties.Headers.ContainsKey("NServiceBus.EnclosedMessageTypes"))
                {
                    var messageType = Encoding.UTF8.GetString((byte[])ea.BasicProperties.Headers["NServiceBus.EnclosedMessageTypes"]);

                    if (messageType.Contains("MyMessage"))
                    {
                        Console.WriteLine($"Found matching message type: {messageType}");

                        // Republish the message to the target queue
                        channel.BasicPublish(
                            exchange: "",
                            routingKey: TargetQueueName,
                            basicProperties: ea.BasicProperties,
                            body: body
                        );

                        Console.WriteLine($"Message republished to {TargetQueueName}");
                    }
                    else
                    {
                        Console.WriteLine($"Skipping message of type: {messageType}");
                    }
                }
                else
                {
                    Console.WriteLine("Message type not found in headers.");
                }

                // Acknowledge the message to remove it from the error queue
                channel.BasicAck(ea.DeliveryTag, multiple: false);
            };

            // Start consuming messages
            channel.BasicConsume(queue: ErrorQueueName, autoAck: false, consumer: consumer);

            Console.WriteLine("Processing error queue. Press Enter to exit.");
            Console.ReadLine();
        }
    }
}

