using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Restaurant.Services.OrderAPI.Messages;
using Restaurant.Services.OrderAPI.Models;
using Restaurant.Services.OrderAPI.Repository;
using System.Text;

namespace Restaurant.Services.OrderAPI.Messaging
{
    public class RabbitMQCheckoutConsumer : BackgroundService
    {
        private readonly OrderRepository _orderRepository;
        private readonly IConnection _connection;
        private readonly IModel _channel;

        public RabbitMQCheckoutConsumer(OrderRepository orderRepository)
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
            _channel.QueueDeclare(queue: "checkoutqueue", false, false, false, arguments: null);
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            stoppingToken.ThrowIfCancellationRequested();

            EventingBasicConsumer consumer = new EventingBasicConsumer(_channel);
            consumer.Received += (ch, ea) =>
            {
                string content = Encoding.UTF8.GetString(ea.Body.ToArray());
                CheckoutHeaderDto checkoutHeaderDto = JsonConvert.DeserializeObject<CheckoutHeaderDto>(content);
                HandleMessage(checkoutHeaderDto);

                _channel.BasicAck(ea.DeliveryTag, false);
            };

            _channel.BasicConsume("checkoutqueue", false, consumer);

            return Task.CompletedTask;
        }

        private async Task HandleMessage(CheckoutHeaderDto checkoutHeaderDto)
        {
            OrderHeader orderHeader = new()
            {
                UserId = checkoutHeaderDto.UserId,
                CouponCode = checkoutHeaderDto.CouponCode,
                OrderTotal = checkoutHeaderDto.OrderTotal,
                DiscountTotal = checkoutHeaderDto.DiscountTotal,
                FirstName = checkoutHeaderDto.FirstName,
                LastName = checkoutHeaderDto.LastName,
                PickupDateTime = checkoutHeaderDto.PickupDateTime,
                OrderTime = DateTime.Now,
                Phone =checkoutHeaderDto.Phone,
                EmailAddress = checkoutHeaderDto.EmailAddress,
                CardNumber = checkoutHeaderDto.CardNumber,
                CVV = checkoutHeaderDto.CVV,
                ExpiryMonthYear = checkoutHeaderDto.ExpiryMonthYear,
                CartTotalItems = checkoutHeaderDto.CartTotalItems,
                OrderDetails = new List<OrderDetails>(),
                PaymentStatus = false
            };

            foreach (CartDetailDto cartDetail in checkoutHeaderDto.CartDetails)
            {
                orderHeader.CartTotalItems += cartDetail.Count;
                orderHeader.OrderDetails.Add(new()
                {
                    ProductId = cartDetail.ProductId,
                    ProductName = cartDetail.Product.Name,
                    Price = cartDetail.Product.Price,
                    Count = cartDetail.Count
                });
            }

            await _orderRepository.AddOrder(orderHeader);

            PaymentRequestMessage paymentRequestMessage = new()
            {
                OrderId = orderHeader.OrderHeaderId,
                Name = $"{orderHeader.FirstName} {orderHeader.LastName}",
                CardNumber = orderHeader.CardNumber,
                CVV = orderHeader.CVV,
                ExpiryMonthYear = orderHeader.ExpiryMonthYear,
                OrderTotal = orderHeader.OrderTotal,
                Email = orderHeader.EmailAddress
            };

            try
            {
                //await _messageBus.PublishMessage(paymentRequestMessage, _paymentMessageTopic);
                //await args.CompleteMessageAsync(args.Message);
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
