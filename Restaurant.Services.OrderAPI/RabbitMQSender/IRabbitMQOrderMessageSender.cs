﻿using Restaurant.MessageBus;

namespace Restaurant.Services.OrderAPI.RabbitMQSender
{
    public interface IRabbitMQOrderMessageSender
    {
        void SendMessage(BaseMessage message, string queueName);
    }
}
