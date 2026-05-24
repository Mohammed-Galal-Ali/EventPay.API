using EventPay.API.Data;
using Microsoft.EntityFrameworkCore;

namespace EventPay.API.Services.Tickets
{
    public class TicketService : ITicketService
    {
        private readonly AppDbContext _context;
        public TicketService(AppDbContext context)
        {
            _context = context;
        }
        public async Task<IEnumerable<object>> GetTicketsByEmailAsync(string email)
        {
            return await _context.Tickets
         .Include(t => t.Event)
         .Where(t => t.BuyerEmail == email)
         .OrderByDescending(t => t.CreatedAt)
         .Select(t => new
         {
             t.Id,
             t.BuyerName,
             EventTitle = t.Event.Title,
             EventDate = t.Event.Date,
             EventLocation = t.Event.Location,
             EventMapLink = t.Event.MapLink,
             t.PricePaid,
             Status = t.Status.ToString(),
             t.CreatedAt
         })
         .ToListAsync();
        }
    }
}
