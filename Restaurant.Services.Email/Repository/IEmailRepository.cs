using Restaurant.Services.Email.Messages;

namespace Restaurant.Repository
{
    public interface IEmailRepository
    {
        Task SendAndLogEmail(UpdatePaymentResultMessage message);
    }
}
