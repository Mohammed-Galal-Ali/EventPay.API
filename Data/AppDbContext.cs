using EventPay.API.Models;
using Microsoft.EntityFrameworkCore;

namespace EventPay.API.Data
{
    public class AppDbContext:DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
            
        }
        public DbSet<Event> Events { get; set; }  
        public DbSet<Ticket> Tickets { get; set; }
        public DbSet<OtpCode> OtpCodes { get; set; }

        }
}
