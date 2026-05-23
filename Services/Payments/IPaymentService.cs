using EventPay.API.DTOs.Payments;

namespace EventPay.API.Services.Payments
{
    public interface IPaymentService
    {
        Task<PaymentResponseDto> CreatePaymentAsync(CreatePaymentDto dto);
        Task HandleWebhookAsync(string paylload, string stripeSignature);

    }
}
