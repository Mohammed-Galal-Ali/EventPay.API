using EventPay.API.DTOs.Events;
using EventPay.API.Services.IEvent;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace EventPay.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EventsController : ControllerBase
    {
        private readonly IEventService _eventService;
        private readonly IValidator<CreateEventDto> _validator;
        public EventsController(IEventService eventService, IValidator<CreateEventDto> validator)
        {
            _eventService = eventService;
            _validator = validator;
        }
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var events = await _eventService.GetAllEventsAsync();
            return Ok(events);
        }
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var eventItem = await _eventService.GetEventByIdAsync(id);
            if (eventItem == null)
            {
                return NotFound(new { message = "Event not found" });
            }
            return Ok(eventItem);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create(CreateEventDto dto)
        {
            var validation = await _validator.ValidateAsync(dto);
            if (!validation.IsValid)
            {
                var errors = validation.Errors
                    .Select(e => e.ErrorMessage)
                    .ToList();
                return BadRequest(new { errors });
            }

            var createdEvent = await _eventService.CreateEventAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = createdEvent.Id }, createdEvent);
        }
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _eventService.DeleteEventAsync(id);
            if (!result)
                return NotFound(new { message = "Event not found" });

            return Ok(new { message = "Event deleted successfully" });
        }
    }
}
