namespace EventPay.API.DTOs.Payments
{
    public class CreatePaymentDto
    {
        public int EventId { get; set; }
        public string BuyerName { get; set; } = string.Empty;
        public string BuyerEmail { get; set; } = string.Empty;
        public string BuyerPhone { get; set; } = string.Empty;
    }
}
