using NServiceBus;


public class InitializeCustomFaultHandlingBehavior : INeedInitialization
{
    public void Customize(EndpointConfiguration busConfiguration)
    {
        busConfiguration.Pipeline.Replace("MoveFaultsToErrorQueueBehavior", typeof(CustomErrorHandlingBehavior), "Custom policy for handling messages instead of default SLR action");
    }
}

