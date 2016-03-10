using System;
using System.Threading.Tasks;
using NServiceBus;
using NServiceBus.Pipeline;


class CustomErrorHandlingBehaviour : Behavior<ITransportReceiveContext>
{
    public override async Task Invoke(ITransportReceiveContext context, Func<Task> next)
    {
        try
        {
            await next().ConfigureAwait(false);
        }
        catch (MessageDeserializationException)
        {
            Console.WriteLine("CustomFaultManager - SerializationFailedForMessage");
        }
        catch (Exception)
        {
            // what is "Processing Always Fails For Message"? Not including SLR doesn't seem obvious for me, so it should be up to the user whether to include SLR or not.
            Console.WriteLine("CustomFaultManager - ProcessingAlwaysFailsForMessage");
            
            // do not rethrow the message if you want to mark the message as processed.
            // rethrow to move the message to the error queue.
            //throw;

            // if you want to rollback the receive operation instead of mark as processed:
            //context.AbortReceiveOperation();
        }
    }
}


class NewMessageProcessingPipelineStep : RegisterStep
{
    public NewMessageProcessingPipelineStep()
        : base("CustomErrorHandlingBehaviour", typeof(CustomErrorHandlingBehaviour), "Adds custom error behavior to pipeline")
    {
        InsertAfter("MoveFaultsToErrorQueue");

        // only invoke this behavior if FLR fails to handle the message
        InsertBeforeIfExists("FirstLevelRetries");

        // if you want to handle the message before it is moved to error queue, insert after SLR.
        // if you want to handle the message before it is handled by SLR, insert it before SLR.
        InsertBeforeIfExists("SecondLevelRetries");
    }
}

class RegisterCustomErrorHandling : INeedInitialization
{
    public void Customize(EndpointConfiguration configuration)
    {
        configuration.Pipeline.Register<NewMessageProcessingPipelineStep>();
    }
}