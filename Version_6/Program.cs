using System;
using System.Threading.Tasks;
using NServiceBus;
using NServiceBus.Features;
using NServiceBus.Logging;

static class Program
{
    static void Main()
    {
        AsyncMain().GetAwaiter().GetResult();
    }

    static async Task AsyncMain()
    {
        Console.Title = "Samples.ErrorHandling.WithSLR";
        LogManager.Use<DefaultFactory>()
            .Level(LogLevel.Warn);

        EndpointConfiguration endpointConfiguration = new EndpointConfiguration("Samples.CustomFaultManager.v6");
        endpointConfiguration.UseSerialization<JsonSerializer>();
        endpointConfiguration.UsePersistence<InMemoryPersistence>();
        endpointConfiguration.EnableInstallers();
        endpointConfiguration.SendFailedMessagesTo("error");
        // if you don't want SLR turn them off
        //endpointConfiguration.DisableFeature<SecondLevelRetries>();

        IEndpointInstance endpoint = await Endpoint.Start(endpointConfiguration);
        try
        {
            Console.WriteLine("Press enter to send a message that will throw an exception.");
            Console.WriteLine("Press any key to exit");

            while (true)
            {
                ConsoleKeyInfo key = Console.ReadKey();
                if (key.Key != ConsoleKey.Enter)
                {
                    return;
                }
                var m = new MyMessage
                {
                    Id = Guid.NewGuid()
                };
                await endpoint.SendLocal(m);
            }
        }
        finally
        {
            await endpoint.Stop();
        }
    }
}