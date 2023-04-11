using Restaurant.Services.OrderAPI.Messaging;

namespace Restaurant.Services.OrderAPI.Extensions
{
    public static class ApplicationBuilderExtensions
    {
        public static IAzureServiceBusConsumer Consumer { get; set; }

        public static IApplicationBuilder UseAzureServiceBusConsumer(this IApplicationBuilder application)
        {
            Consumer = application.ApplicationServices.GetService<IAzureServiceBusConsumer>();

            IHostApplicationLifetime hostApplicationLife = application.ApplicationServices.GetService<IHostApplicationLifetime>();
            hostApplicationLife.ApplicationStarted.Register(OnStart);
            hostApplicationLife.ApplicationStopped.Register(OnStop);

            return application;
        }

        private static void OnStop() => Consumer.Stop();
        private static void OnStart() => Consumer.Start();
    }
}
