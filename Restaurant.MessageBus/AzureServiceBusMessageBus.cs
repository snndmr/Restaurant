using Azure.Messaging.ServiceBus;
using Newtonsoft.Json;

namespace Restaurant.MessageBus
{
    public class AzureServiceBusMessageBus : IMessageBus
    {
        private readonly string CheckoutTopicMessage = "Endpoint=sb://restaurantservices.servicebus.windows.net/;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=ICIC1G7H0HbGUIoA2ubvdDS4Eo4/M/DyQ+ASbPRtbnc=;EntityPath=checkouttopicmessage";
        private readonly string OrderPaymentProcessTopic = "Endpoint=sb://restaurantservices.servicebus.windows.net/;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=9t6g1QYAABKjweM+FUyN4ukqTSfyjY6Vy+ASbNp9xA8=;EntityPath=orderpaymentprocesstopic";

        public async Task PublishMessage(BaseMessage message, string topicName)
        {
            await using ServiceBusClient client = new(topicName == "checkouttopicmessage" ? CheckoutTopicMessage : OrderPaymentProcessTopic);

            string messageAsJson = JsonConvert.SerializeObject(message);
            ServiceBusMessage serviceBusMessage = new(messageAsJson);

            ServiceBusSender sender = client.CreateSender(topicName);
            await sender.SendMessageAsync(serviceBusMessage);

            await sender.CloseAsync();
            await client.DisposeAsync();
        }
    }
}
