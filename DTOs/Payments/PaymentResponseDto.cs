namespace EventPay.API.DTOs.Payments
{
    public class PaymentResponseDto
    {
        public int TicketId { get; set; }
        public string ClientSecret { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public string EventTitle { get; set; } = string.Empty;
    }
}
