namespace EventPay.API.DTOs.Events
{
    public class CreateEventDto
    {
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public DateTime Date { get; set; }
        public string Location { get; set; } = string.Empty;
        public decimal TicketPrice { get; set; }
        public int TotalTickets { get; set; }
    }
}
