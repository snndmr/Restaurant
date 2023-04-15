using PaymentProcessor;
using Restaurant.MessageBus;
using Restaurant.Services.PaymentAPI.Extensions;
using Restaurant.Services.PaymentAPI.Messaging;
using Restaurant.Services.PaymentAPI.RabbitMQSender;

namespace Restaurant.Services.PaymentAPI
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllers();

            builder.Services.AddSingleton<IProcessPayment, ProcessPayment>();
            builder.Services.AddSingleton<IAzureServiceBusConsumer, AzureServiceBusConsumer>();
            builder.Services.AddSingleton<IMessageBus, AzureServiceBusMessageBus>();
            builder.Services.AddSingleton<IRabbitMQPaymentMessageSender, RabbitMQPaymentMessageSender>();
            builder.Services.AddHostedService<RabbitMQPaymentConsumer>();

            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();


            app.MapControllers();

            app.UseAzureServiceBusConsumer();
            app.Run();
        }
    }
}