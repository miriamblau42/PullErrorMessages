// Define a message
public class MyMessage : IMessage
{
    public Guid Id { get; set; }
    public string Name { get; set; }
}

// Define the handler
public class MyMessageHandler : IHandleMessages<MyMessage>
{
    public Task Handle(MyMessage message, IMessageHandlerContext context)
    {
        throw new NotImplementedException();
    }
}
