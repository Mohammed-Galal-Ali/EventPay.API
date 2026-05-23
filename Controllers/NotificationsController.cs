using EventPay.API.DTOs.Notifications;
using EventPay.API.Models;
using EventPay.API.Services.Messaging;
using Microsoft.AspNetCore.Mvc;

namespace EventPay.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class NotificationsController : ControllerBase
    {
        private readonly OtpService _otpService;
        private readonly IMessageService _messageService;

        public NotificationsController(OtpService otpService, IMessageService messageService)
        {
            _otpService = otpService;
            _messageService = messageService;
        }

        [HttpPost("send-telegram")]
        public async Task<IActionResult> SendTelegram([FromBody] SendTelegramDto dto)
        {
            await _messageService.SendTelegramAsync(dto.ChatId, dto.Message);
            return Ok(new { message = "اتبعت بنجاح!" });
        }

        [HttpPost("send-otp/telegram")]
        public async Task<IActionResult> SendOtpTelegram([FromBody] SendOtpDto dto)
        {
            var code = await _otpService.SendOtpAsync(dto.Recipient, OtpType.Telegram);
            return Ok(new { message = "OTP اتبعت!", code });
        }

        [HttpPost("verify-otp")]
        public async Task<IActionResult> VerifyOtp([FromBody] VerifyOtpDto dto)
        {
            var isValid = await _otpService.VerifyOtpAsync(dto.Recipient, dto.Code);

            if (!isValid)
                return BadRequest(new { message = "الكود غلط أو انتهت صلاحيته" });

            return Ok(new { message = "✅ الكود صحيح!" });
        }

        [HttpPost("send-email")]
        public async Task<IActionResult> SendEmail([FromBody] SendEmailDto dto)
        {
            await _messageService.SendEmailAsync(dto.To, dto.Subject, dto.Body);
            return Ok(new { message = "الإيميل اتبعت!" });
        }
        [HttpPost("send-sms")]
        public async Task<IActionResult> SendSms([FromBody] SendSmsDto dto)
        {
            await _messageService.SendSmsAsync(dto.To, dto.Message);
            return Ok(new { message = "SMS اتبعت!" });
        }

        [HttpPost("send-whatsapp")]
        public async Task<IActionResult> SendWhatsApp([FromBody] SendWhatsAppDto dto)
        {
            await _messageService.SendWhatsAppAsync(dto.To, dto.Message);
            return Ok(new { message = "WhatsApp اتبعت!" });
        }
    }
}