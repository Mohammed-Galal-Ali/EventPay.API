namespace EventPay.API.Models
{
    public class Ticket
    {
        public int Id { get; set; }
        public string BuyerName { get; set; } = string.Empty;
        public string BuyerEmail { get; set; } = string.Empty;
        public string BuyerPhone { get; set;} = string.Empty;
        public int EventId { get; set; }
        public Event Event { get; set; } = null!;
        public decimal PricePaid { get; set; }
        public TicketStatus Status { get; set; } = TicketStatus.Pending;
        public string PaymentIntentId { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    }
    public enum TicketStatus
    {
        Pending,    // لسه مدفعش
        Paid,       // دفع
        Cancelled   // اتلغى
    }
}
