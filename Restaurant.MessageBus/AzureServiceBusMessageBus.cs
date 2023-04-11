using Azure.Messaging.ServiceBus;
using Newtonsoft.Json;

namespace Restaurant.MessageBus
{
    public class AzureServiceBusMessageBus : IMessageBus
    {
        private readonly string connectionString = "Endpoint=sb://restaurantservices.servicebus.windows.net/;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=GvNSi7DA2v6G2R4/BA3e78vrvtCqxqz9l+ASbLOP2lM=";

        public async Task PublishMessage(BaseMessage message, string topicName)
        {
            await using ServiceBusClient client = new(connectionString);

            string messageAsJson = JsonConvert.SerializeObject(message);
            ServiceBusMessage serviceBusMessage = new(messageAsJson);

            ServiceBusSender sender = client.CreateSender(topicName);
            await sender.SendMessageAsync(serviceBusMessage);

            await sender.CloseAsync();
            await client.DisposeAsync();
        }
    }
}
