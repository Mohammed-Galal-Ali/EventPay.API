using EventPay.API.DTOs.Events;

namespace EventPay.API.Services.IEvent
{
    public interface IEventService
    {
        Task<List<EventResponseDto>> GetAllEventsAsync();
        Task<EventResponseDto?> GetEventByIdAsync(int id);
        Task<EventResponseDto> CreateEventAsync(CreateEventDto dto);
    }
}
