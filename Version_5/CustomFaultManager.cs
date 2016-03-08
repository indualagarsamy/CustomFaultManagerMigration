using System;
using NServiceBus;
using NServiceBus.Faults;

public class CustomFaultManager : IManageMessageFailures, INeedInitialization
{
    public void SerializationFailedForMessage(TransportMessage message, Exception e)
    {
        Console.WriteLine("CustomFaultManager - SerializationFailedForMessage");
    }

    public void ProcessingAlwaysFailsForMessage(TransportMessage message, Exception e)
    {
        Console.WriteLine("CustomFaultManager - ProcessingAlwaysFailsForMessage");
    }

    public void Init(Address address)
    {
        Console.WriteLine("CustomFaultManager - Init");
    }
        
    public void Customize(BusConfiguration configuration)
    {
        configuration.RegisterComponents(c => c.ConfigureComponent<CustomFaultManager>(DependencyLifecycle.InstancePerCall));
    }
}

