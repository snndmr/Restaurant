using Microsoft.EntityFrameworkCore;
using Restaurant.Repository;
using Restaurant.Services.Email.DbContexts;
using Restaurant.Services.Email.Extensions;
using Restaurant.Services.Email.Messaging;
using Restaurant.Services.Email.Repository;

namespace Restaurant.Services.Email
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddDbContext<ApplicationDbContext>(options =>
                options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));
            builder.Services.AddScoped<IEmailRepository, EmailRepository>();

            var optionBuilder = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"));
            builder.Services.AddSingleton(new EmailRepository(optionBuilder.Options));
            builder.Services.AddSingleton<IAzureServiceBusConsumer, AzureServiceBusConsumer>();
            builder.Services.AddHostedService<RabbitMQPaymentConsumer>();

            builder.Services.AddControllers();
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