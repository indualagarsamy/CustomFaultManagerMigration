//using System;
//using System.Threading.Tasks;
//using NServiceBus;
//using NServiceBus.Pipeline;
//    
//public class CustomErrorHandlingBehavior : ForkConnector<ITransportReceiveContext, IFaultContext>
//{
//    CriticalError criticalError;
//    string errorQueueAddress;
//    string localAddress;
//    BusNotifications notifications;
//
//    public CustomErrorHandlingBehavior(CriticalError criticalError, BusNotifications notifications, string errorQueueAddress, string localAddress)
//    {
//        this.criticalError = criticalError;
//        this.notifications = notifications;
//        this.errorQueueAddress = errorQueueAddress;
//        this.localAddress = localAddress;
//    }
//    public override async Task Invoke(ITransportReceiveContext context, Func<Task> next, Func<IFaultContext, Task> fork)
//    {
//        try
//        {
//            await next().ConfigureAwait(false);
//        }
//        catch (Exception exception)
//        {
//            try
//            {
//                var message = context.Message;
//                Console.WriteLine($"MyOwnFaultManager - Processing always fails for message '{message.MessageId}' due to an exception:", exception);
//            }
//            catch (Exception ex)
//            {
//                criticalError.Raise("Failed to process Custom Policy for errors", ex);
//                throw;
//            }
//        }
//    }
//}
//
