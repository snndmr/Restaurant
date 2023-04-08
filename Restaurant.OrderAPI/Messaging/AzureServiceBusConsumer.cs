using Azure.Messaging.ServiceBus;
using Newtonsoft.Json;
using Restaurant.OrderAPI.Messages;
using Restaurant.OrderAPI.Models;
using Restaurant.OrderAPI.Repository;
using System.Text;

namespace Restaurant.OrderAPI.Messaging
{
    public class AzureServiceBusConsumer : IAzureServiceBusConsumer
    {
        private readonly OrderRepository _orderRepository;
        private readonly IConfiguration _configuration;

        private readonly string _serviceBusConnectionString;
        private readonly string _checkoutMessageTopic;
        private readonly string _subscriptionCheckOut;

        private readonly ServiceBusProcessor _checkOutProcessor;

        public AzureServiceBusConsumer(OrderRepository orderRepository, IConfiguration configuration)
        {
            _orderRepository = orderRepository;
            _configuration = configuration;

            _serviceBusConnectionString = _configuration.GetValue<string>("ServiceBusConnectionStrings");
            _checkoutMessageTopic = _configuration.GetValue<string>("CheckoutMessageTopic");
            _subscriptionCheckOut = _configuration.GetValue<string>("SubscriptionCheckOut");

            ServiceBusClient client = new ServiceBusClient(_serviceBusConnectionString);
            _checkOutProcessor = client.CreateProcessor(_checkoutMessageTopic, _subscriptionCheckOut);
        }

        public async Task Start()
        {
            _checkOutProcessor.ProcessMessageAsync += OnCheckOutMessageReceived;
            _checkOutProcessor.ProcessErrorAsync += ErrorHandler;
            await _checkOutProcessor.StartProcessingAsync();
        }

        public async Task Stop()
        {
            await _checkOutProcessor.StopProcessingAsync();
            await _checkOutProcessor.DisposeAsync();
        }

        private Task ErrorHandler(ProcessErrorEventArgs args)
        {
            Console.WriteLine(args.Exception.ToString());
            return Task.CompletedTask;
        }

        private async Task OnCheckOutMessageReceived(ProcessMessageEventArgs args)
        {
            ServiceBusReceivedMessage message = args.Message;
            string body = Encoding.UTF8.GetString(message.Body);

            CheckoutHeaderDto checkoutHeaderDto = JsonConvert.DeserializeObject<CheckoutHeaderDto>(body);

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
        }
    }
}
