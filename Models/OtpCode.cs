namespace EventPay.API.Models
{
    public class OtpCode
    {
        public int Id { get; set; }

        // لمين الكود ده؟ (email, phone, etc.)
        public string Recipient { get; set; } = string.Empty;

        // الكود نفسه
        public string Code { get; set; } = string.Empty;

        // نوع الإرسال
        public OtpType Type { get; set; }

        // هينتهي امتى؟
        public DateTime ExpiresAt { get; set; }

        // اتستخدم قبل كده؟
        public bool IsUsed { get; set; } = false;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }

    public enum OtpType
    {
        Email,
        SMS,
        WhatsApp,
        Telegram
    }
}