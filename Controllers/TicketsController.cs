using EventPay.API.DTOs;
using EventPay.API.DTOs.Notifications;
using EventPay.API.DTOs.Tickets;
using EventPay.API.Models;
using EventPay.API.Services.Messaging;
using EventPay.API.Services.Tickets;
using Microsoft.AspNetCore.Mvc;

namespace EventPay.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TicketsController : ControllerBase
    {
        private readonly ITicketService _ticketService;
        private readonly OtpService _otpService;

        public TicketsController(ITicketService ticketService, OtpService otpService)
        {
            _ticketService = ticketService;
            _otpService = otpService;
        }

        [HttpPost("request-otp")]
        public async Task<IActionResult> RequestOtp([FromBody] SendOtpDto dto)
        {
            await _otpService.SendOtpAsync(dto.Recipient, OtpType.Email);
            return Ok(new { message = "OTP sent to your email" });
        }

        [HttpPost("my-tickets")]
        public async Task<IActionResult> GetMyTickets([FromBody] GetMyTicketsDto dto)
        {
            var isValid = await _otpService.VerifyOtpAsync(dto.Email, dto.OtpCode);
            if (!isValid)
                return BadRequest(new { message = "Invalid or expired OTP" });

            var tickets = await _ticketService.GetTicketsByEmailAsync(dto.Email);
            return Ok(tickets);
        }
    }
}