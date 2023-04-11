using Azure.Messaging.ServiceBus;
using Newtonsoft.Json;
using Restaurant.MessageBus;
using Restaurant.Services.OrderAPI.Messages;
using Restaurant.Services.OrderAPI.Models;
using Restaurant.Services.OrderAPI.Repository;
using System.Text;

namespace Restaurant.Services.OrderAPI.Messaging
{
    public class AzureServiceBusConsumer : IAzureServiceBusConsumer
    {
        private readonly OrderRepository _orderRepository;
        private readonly IConfiguration _configuration;
        private readonly IMessageBus _messageBus;

        private readonly string _serviceBusConnectionString;
        private readonly string _checkoutMessageTopic;
        private readonly string _paymentMessageTopic;
        private readonly string _subscriptionCheckOut;

        private readonly ServiceBusProcessor _checkOutProcessor;

        public AzureServiceBusConsumer(OrderRepository orderRepository, IConfiguration configuration, IMessageBus messageBus)
        {
            _orderRepository = orderRepository;
            _configuration = configuration;
            _messageBus = messageBus;

            _serviceBusConnectionString = _configuration.GetValue<string>("ServiceBusConnectionStrings");
            _checkoutMessageTopic = _configuration.GetValue<string>("CheckoutMessageTopic");
            _paymentMessageTopic = _configuration.GetValue<string>("OrderPaymentProcessTopic");
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

            PaymentRequestMessage paymentRequestMessage = new()
            {
                OrderId = orderHeader.OrderHeaderId,
                Name = $"{orderHeader.FirstName} {orderHeader.LastName}",
                CardNumber = orderHeader.CardNumber,
                CVV = orderHeader.CVV,
                ExpiryMonthYear = orderHeader.ExpiryMonthYear,
                OrderTotal = orderHeader.OrderTotal
            };

            try
            {
                await _messageBus.PublishMessage(paymentRequestMessage, _paymentMessageTopic);
                await args.CompleteMessageAsync(args.Message);
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
