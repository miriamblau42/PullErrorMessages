using NServiceBus;
using System;
using System.Threading.Tasks;

namespace ErrorQueueProcessor
{
    public class Message
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
    }
    public class MessageHandler : IHandleMessages<Message>
    {
        public Task Handle(Message message, IMessageHandlerContext context)
        {
            throw new NotImplementedException();
        }
    }
}
