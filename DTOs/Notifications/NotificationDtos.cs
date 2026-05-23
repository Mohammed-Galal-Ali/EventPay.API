namespace EventPay.API.DTOs.Notifications
{
    public class SendTelegramDto
    {
        public string ChatId { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
    }

    public class SendOtpDto
    {
        public string Recipient { get; set; } = string.Empty;
    }

    public class VerifyOtpDto
    {
        public string Recipient { get; set; } = string.Empty;
        public string Code { get; set; } = string.Empty;
    }
    public class SendEmailDto
    {
        public string To { get; set; } = string.Empty;
        public string Subject { get; set; } = string.Empty;
        public string Body { get; set; } = string.Empty;
    }
    public class SendSmsDto
    {
        public string To { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
    }

    public class SendWhatsAppDto
    {
        public string To { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
    }

}