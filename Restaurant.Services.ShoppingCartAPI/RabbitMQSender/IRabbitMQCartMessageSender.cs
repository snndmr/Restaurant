using Restaurant.MessageBus;

namespace Restaurant.Services.ShoppingCartAPI.RabbitMQSender
{
    public interface IRabbitMQCartMessageSender
    {
        void SendMessage(BaseMessage message, string queueName);
    }
}
