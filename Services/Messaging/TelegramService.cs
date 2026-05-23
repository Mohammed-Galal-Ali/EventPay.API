namespace EventPay.API.Services.Messaging
{
    public class TelegramService
    {
        private readonly IConfiguration _config;
        private readonly HttpClient _httpClient;

        public TelegramService(IConfiguration config, HttpClient httpClient)
        {
            _config = config;
            _httpClient = httpClient;
        }

        public async Task SendTelegramAsync(string chatId, string message)
        {
            var botToken = _config["Telegram:BotToken"];
            var url = $"https://api.telegram.org/bot{botToken}/sendMessage";

            var payload = new
            {
                chat_id = chatId,
                text = message,
                parse_mode = "HTML"
            };

            await _httpClient.PostAsJsonAsync(url, payload);
        }
    }
}