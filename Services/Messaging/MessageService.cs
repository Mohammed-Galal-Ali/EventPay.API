namespace EventPay.API.Services.Messaging
{
    public class MessageService : IMessageService
    {
        private readonly EmailService _emailService;
        private readonly TwilioService _twilioService;
        private readonly TelegramService _telegramService;

        public MessageService(
            EmailService emailService,
            TwilioService twilioService,
            TelegramService telegramService)
        {
            _emailService = emailService;
            _twilioService = twilioService;
            _telegramService = telegramService;
        }

        public async Task SendEmailAsync(string to, string subject, string body)
            => await _emailService.SendEmailAsync(to, subject, body);

        public async Task SendSmsAsync(string to, string message)
            => await _twilioService.SendSmsAsync(to, message);

        public async Task SendWhatsAppAsync(string to, string message)
            => await _twilioService.SendWhatsAppAsync(to, message);

        public async Task SendTelegramAsync(string chatId, string message)
            => await _telegramService.SendTelegramAsync(chatId, message);
    }
}