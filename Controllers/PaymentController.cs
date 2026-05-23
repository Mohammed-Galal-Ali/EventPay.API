using EventPay.API.DTOs.Payments;
using EventPay.API.Services.Payments;
using FluentValidation;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace EventPay.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PaymentController : ControllerBase
    {
        private readonly IPaymentService _paymentService;
        private readonly IValidator<CreatePaymentDto> _validator;
        public PaymentController(IPaymentService paymentService, IValidator<CreatePaymentDto> validator)
        {
            _paymentService = paymentService;
            _validator = validator;
        }
        [HttpPost]
        public async Task<IActionResult> CreatePayment([FromBody] CreatePaymentDto dto)
        {
            var validation = await _validator.ValidateAsync(dto);
            if (!validation.IsValid)
            {
                var errors = validation.Errors
                    .Select(e => e.ErrorMessage)
                    .ToList();
                return BadRequest(new { errors });
            }

            var result = await _paymentService.CreatePaymentAsync(dto);
            return Ok(result);
        }
        [HttpPost("webhook")]
        public async Task<IActionResult> HandleWebhook()
        {
            var payload = await new StreamReader(HttpContext.Request.Body).ReadToEndAsync();
            var stripeSignature = Request.Headers["Stripe-Signature"];

            try
            {
                await _paymentService.HandleWebhookAsync(payload, stripeSignature);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
