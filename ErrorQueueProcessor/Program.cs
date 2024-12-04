using NServiceBus;
using System;

namespace ErrorQueueProcessor
{
    public class Program
    {
        public IEndpointInstance Endpoint { get; set; }

        //public Program(IEndpointInstance endpoint)
        //{
        //    Endpoint = endpoint;
        //    Endpoint.Send(new Message { Name = "test1", Id = Guid.NewGuid() }); 
        //}
    }
}
