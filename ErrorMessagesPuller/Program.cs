class Program
{
    static void Main(string[] args)
    {
        var processor = new ErrorQueueProcessor();
        processor.ProcessErrorQueue();
    }
}
