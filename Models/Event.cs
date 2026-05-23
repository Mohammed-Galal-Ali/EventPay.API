namespace EventPay.API.Models
{
    public class Event
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public DateTime Date { get; set; } 
        public string Location { get; set; } = string.Empty;
        public decimal TicketPrice { get; set; }
        public int TotalTickets { get; set; }
        public int SoldTickets { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public double? Latitude { get; set; }
        public double? Longitude { get; set; }
        public string? MapLink { get; set; }
        public List<Ticket> Tickets { get; set; } = new ();
    } 
}
