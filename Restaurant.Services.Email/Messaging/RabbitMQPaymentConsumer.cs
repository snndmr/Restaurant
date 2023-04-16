using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Restaurant.Services.Email.Messages;
using Restaurant.Services.Email.Repository;
using System.Text;

namespace Restaurant.Services.Email.Messaging
{
    public class RabbitMQPaymentConsumer : BackgroundService
    {
        private const string ExchangeName = "PublishSubscribePaymentUpdate_Exchange";

        private readonly EmailRepository _emailRepository;
        private readonly IConnection _connection;
        private readonly IModel _channel;

        private readonly string queueName = string.Empty;

        public RabbitMQPaymentConsumer(EmailRepository emailRepository)
        {
            _emailRepository = emailRepository;

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
                await _emailRepository.SendAndLogEmail(updatePayment);
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
