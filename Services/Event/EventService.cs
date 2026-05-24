using EventPay.API.Data;
using EventPay.API.DTOs.Events;
using EventPay.API.Models;
using EventPay.API.Services.IEvent;
using EventPay.API.Services.Maps;
using Microsoft.EntityFrameworkCore;
namespace EventPay.API.Services.EventService
{
    public class EventService : IEventService
    {
        private readonly AppDbContext _context;
        private readonly IMapsService _mapsService;
        public EventService(AppDbContext context, IMapsService mapsService)
        {
            _context = context;
            _mapsService = mapsService;
        }
        public async Task<EventResponseDto> CreateEventAsync(CreateEventDto dto)
        {
            var geoResult = await _mapsService.GeocodeAsync(dto.Location);
            var newEvent = new Event
            {
                Title = dto.Title,
                Description = dto.Description,
                Date = dto.Date,
                Location = dto.Location,
                TicketPrice = dto.TicketPrice,
                TotalTickets = dto.TotalTickets,
                SoldTickets = 0,

                // ✅ بنحفظ الـ coordinates لو الـ Geocoding نجح
                Latitude = geoResult?.Latitude,
                Longitude = geoResult?.Longitude,
                MapLink = geoResult?.MapLink
            };

            _context.Events.Add(newEvent);
            await _context.SaveChangesAsync();

            return new EventResponseDto
            {
                Id = newEvent.Id,
                Title = newEvent.Title,
                Description = newEvent.Description,
                Date = newEvent.Date,
                Location = newEvent.Location,
                TicketPrice = newEvent.TicketPrice,
                TotalTickets = newEvent.TotalTickets,
                SoldTickets = newEvent.SoldTickets,
                Latitude = newEvent.Latitude,
                Longitude = newEvent.Longitude,
                MapLink = newEvent.MapLink
            };
        }

        public async Task<List<EventResponseDto>> GetAllEventsAsync()
        {
            var events = await _context.Events
                .OrderBy(e => e.Date)
                .Select(e => new EventResponseDto
                {
                    Id = e.Id,
                    Title = e.Title,
                    Description = e.Description,
                    Date = e.Date,
                    Location = e.Location,
                    TicketPrice = e.TicketPrice,
                    TotalTickets = e.TotalTickets,
                    SoldTickets = e.SoldTickets,
                       Latitude = e.Latitude,
                    Longitude = e.Longitude,
                    MapLink = e.MapLink
                })
                .ToListAsync();

            return events;
        }

        public async Task<EventResponseDto?> GetEventByIdAsync(int id)
        {
            var e = await _context.Events.FindAsync(id);
            if (e == null) return null;
            return new EventResponseDto
            {
                Id = e.Id,
                Title = e.Title,
                Description = e.Description,
                Date = e.Date,
                Location = e.Location,
                TicketPrice = e.TicketPrice,
                TotalTickets = e.TotalTickets,
                SoldTickets = e.SoldTickets,
                Latitude = e.Latitude,
                Longitude = e.Longitude,
                MapLink = e.MapLink
            };
        }
        public async Task<bool> DeleteEventAsync(int id)
        {
            var eventItem = await _context.Events.FindAsync(id);
            if (eventItem == null) return false;

            _context.Events.Remove(eventItem);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
