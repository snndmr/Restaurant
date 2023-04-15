using Newtonsoft.Json;
using RabbitMQ.Client;
using Restaurant.MessageBus;
using System.Text;

namespace Restaurant.Services.PaymentAPI.RabbitMQSender
{
    public class RabbitMQPaymentMessageSender : IRabbitMQPaymentMessageSender
    {
        private readonly string _hostname;
        private readonly string _username;
        private readonly string _password;
        private IConnection _connection;
        private const string ExchangeName = "PublishSubscribePaymentUpdate_Exchange";

        public RabbitMQPaymentMessageSender()
        {
            _hostname = "localhost";
            _username = "guest";
            _password = "guest";
        }

        public void SendMessage(BaseMessage message)
        {
            if (IsConnectionExist())
            {
                using IModel channel = _connection.CreateModel();

                channel.ExchangeDeclare(ExchangeName, ExchangeType.Fanout, durable: false);

                string messageToJson = JsonConvert.SerializeObject(message);
                byte[] messageToUTF8 = Encoding.UTF8.GetBytes(messageToJson);

                channel.BasicPublish(exchange: ExchangeName, string.Empty, basicProperties: null, body: messageToUTF8);
            }
        }

        private void CreateConnection()
        {
            try
            {
                ConnectionFactory connectionFactory = new()
                {
                    HostName = _hostname,
                    UserName = _username,
                    Password = _password
                };

                _connection = connectionFactory.CreateConnection();
            }
            catch (Exception) { }
        }

        private bool IsConnectionExist()
        {
            if (_connection == null)
            {
                CreateConnection();
            }

            return _connection != null;
        }
    }
}
