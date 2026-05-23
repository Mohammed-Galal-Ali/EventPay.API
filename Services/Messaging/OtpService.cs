using EventPay.API.Data;
using EventPay.API.Models;

namespace EventPay.API.Services.Messaging
{
    public class OtpService
    {
        private readonly AppDbContext _context;
        private readonly IMessageService _messageService;

        public OtpService(AppDbContext context, IMessageService messageService)
        {
            _context = context;
            _messageService = messageService;
        }

        public async Task<string> SendOtpAsync(string recipient, OtpType type)
        {
            // Step 1: نعمل كود عشوائي 6 أرقام
            var code = new Random().Next(100000, 999999).ToString();

            // Step 2: نحفظه في الـ DB
            var otp = new OtpCode
            {
                Recipient = recipient,
                Code = code,
                Type = type,
                ExpiresAt = DateTime.UtcNow.AddMinutes(10)
            };

            _context.OtpCodes.Add(otp);
            await _context.SaveChangesAsync();

            // Step 3: نبعته للـ User
            var message = $"كود التحقق بتاعك هو: {code}\nصالح لمدة 10 دقايق.";

            switch (type)
            {
                case OtpType.Email:
                    await _messageService.SendEmailAsync(recipient, "كود التحقق - EventPay", message);
                    break;
                case OtpType.SMS:
                    await _messageService.SendSmsAsync(recipient, message);
                    break;
                case OtpType.WhatsApp:
                    await _messageService.SendWhatsAppAsync(recipient, message);
                    break;
                case OtpType.Telegram:
                    await _messageService.SendTelegramAsync(recipient, message);
                    break;
            }

            return code; // في الـ Production مش هنرجع الكود!
        }

        public async Task<bool> VerifyOtpAsync(string recipient, string code)
        {
            var otp = _context.OtpCodes
                .Where(o => o.Recipient == recipient
                         && o.Code == code
                         && !o.IsUsed
                         && o.ExpiresAt > DateTime.UtcNow)
                .OrderByDescending(o => o.CreatedAt)
                .FirstOrDefault();

            if (otp == null) return false;

            // نعمله Used عشان محدش يستخدمه تاني
            otp.IsUsed = true;
            await _context.SaveChangesAsync();

            return true;
        }
    }
}