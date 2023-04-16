using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Restaurant.Repository;
using Restaurant.Services.OrderAPI.Messages;
using Restaurant.Services.OrderAPI.Repository;
using System.Text;

namespace Restaurant.Services.OrderAPI.Messaging
{
    public class RabbitMQPaymentConsumer : BackgroundService
    {
        private const string ExchangeName = "PublishSubscribePaymentUpdate_Exchange";

        private readonly IOrderRepository _orderRepository;
        private readonly IConnection _connection;
        private readonly IModel _channel;

        private readonly string queueName = string.Empty;

        public RabbitMQPaymentConsumer(OrderRepository orderRepository)
        {
            _orderRepository = orderRepository;

            ConnectionFactory connectionFactory = new()
            {
                HostName = "localhost",
                UserName = "guest",
                Password = "guest"
            };

            _connection = connectionFactory.CreateConnection();
            _channel = _connection.CreateModel();
            _channel.ExchangeDeclare(ExchangeName, ExchangeType.Fanout);

            queueName = _channel.QueueDeclare().QueueName;
            _channel.QueueBind(queueName, ExchangeName, string.Empty);
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            stoppingToken.ThrowIfCancellationRequested();

            EventingBasicConsumer consumer = new EventingBasicConsumer(_channel);
            consumer.Received += async (ch, ea) =>
            {
                string content = Encoding.UTF8.GetString(ea.Body.ToArray());
                UpdatePaymentResultMessage updatePayment = JsonConvert.DeserializeObject<UpdatePaymentResultMessage>(content);
                await HandleMessage(updatePayment);

                _channel.BasicAck(ea.DeliveryTag, false);
            };

            _channel.BasicConsume(queueName, false, consumer);

            return Task.CompletedTask;
        }

        private async Task HandleMessage(UpdatePaymentResultMessage updatePayment)
        {
            try
            {
                await _orderRepository.UpdateOrderPaymentStatus(updatePayment.OrderId, updatePayment.Status);
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
