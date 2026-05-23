using EventPay.API.Services.Maps;
using Microsoft.AspNetCore.Mvc;

namespace EventPay.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MapsController : ControllerBase
    {
        private readonly IMapsService _mapsService;

        public MapsController(IMapsService mapsService)
        {
            _mapsService = mapsService;
        }

        [HttpGet("geocode")]
        public async Task<IActionResult> Geocode([FromQuery] string address)
        {
            var result = await _mapsService.GeocodeAsync(address);

            if (result == null)
                return NotFound(new { message = "المكان مش موجود" });

            return Ok(result);
        }

        [HttpGet("distance")]
        public IActionResult Distance(
            [FromQuery] double lat1, [FromQuery] double lng1,
            [FromQuery] double lat2, [FromQuery] double lng2)
        {
            var distance = _mapsService.CalculateDistance(lat1, lng1, lat2, lng2);

            return Ok(new
            {
                distanceKm = Math.Round(distance, 2),
                message = $"المسافة {Math.Round(distance, 2)} كيلومتر"
            });
        }
    }
}