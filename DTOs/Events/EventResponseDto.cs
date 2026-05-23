namespace EventPay.API.DTOs.Events
{
    public class EventResponseDto
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public DateTime Date { get; set; }
        public string Location { get; set; } = string.Empty;
        public decimal TicketPrice { get; set; }
        public int TotalTickets { get; set; }
        public int SoldTickets { get; set; }
        public int AvailableTickets => TotalTickets - SoldTickets;

        //  الجديد
        public double? Latitude { get; set; }
        public double? Longitude { get; set; }
        public string? MapLink { get; set; }
    }
}
