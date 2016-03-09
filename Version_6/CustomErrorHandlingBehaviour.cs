using System;
using System.Runtime.Serialization;
using System.Threading.Tasks;
using NServiceBus;
using NServiceBus.Pipeline;


class CustomErrorHandlingBehaviour : Behavior<ITransportReceiveContext>
{
    private readonly CriticalError _criticalError;

    public CustomErrorHandlingBehaviour(CriticalError criticalError)
    {
        _criticalError = criticalError;
    }

    public override async Task Invoke(ITransportReceiveContext context, Func<Task> next)
    {
        var message = context.Message;

        try
        {
            await next().ConfigureAwait(false);
        }
        catch (MessageDeserializationException)
        {
            Console.WriteLine("CustomFaultManager - SerializationFailedForMessage");
        }
        catch (SerializationException)
        {
            Console.WriteLine("CustomFaultManager - SerializationFailedForMessage");
        }
        catch (InvalidOperationException)
        {
            Console.WriteLine("CustomFaultManager - SerializationFailedForMessage");
            Console.WriteLine("CustomFaultManager - ProcessingAlwaysFailsForMessage");
        }
        catch (Exception exception)
        {
            try
            {
                Console.WriteLine(
                    $"MyOwnFaultManager - Processing always fails for message '{message.MessageId}' due to an exception:",
                    exception);
            }
            catch (Exception ex)
            {
                _criticalError.Raise("Failed to process Custom Policy for errors", ex);
                throw;
            }
        }
    }
}


class NewMessageProcessingPipelineStep : RegisterStep
{
    public NewMessageProcessingPipelineStep()
        : base("ForwardMessagesToSqlTransport", typeof(CustomErrorHandlingBehaviour), "Adds custom error behavior to pipeline")
    {
        InsertAfterIfExists("FirstLevelRetries");
        InsertAfterIfExists("SecondLevelRetries");
        InsertAfterIfExists("MoveFaultsToErrorQueue");
    }
}

class RegisterCustomErrorHandling : INeedInitialization
{
    public void Customize(EndpointConfiguration configuration)
    {
        configuration.Pipeline.Register<NewMessageProcessingPipelineStep>();
    }
}