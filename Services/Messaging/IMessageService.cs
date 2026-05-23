namespace EventPay.API.Services.Messaging
{
    public interface IMessageService
    {
        Task SendEmailAsync(string to, string subject, string body);
        Task SendSmsAsync(string to, string message);
        Task SendWhatsAppAsync(string to, string message);

        Task SendTelegramAsync(string chatId, string message);
    }
}
