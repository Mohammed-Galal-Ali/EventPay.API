using System.Text.Json;

namespace EventPay.API.Services.Maps
{
    public class MapsService : IMapsService
    {
        private readonly HttpClient _httpClient;

        public MapsService(HttpClient httpClient)
        {
            _httpClient = httpClient;
            // Nominatim بيطلب User-Agent إلا هيرفض الـ request
            _httpClient.DefaultRequestHeaders.Add("User-Agent", "EventPay/1.0");
        }

        public async Task<GeocodingResult?> GeocodeAsync(string address)
        {
            // Step 1: نبعت الـ request لـ Nominatim
            var url = $"https://nominatim.openstreetmap.org/search?q={Uri.EscapeDataString(address)}&format=json&limit=1";

            var response = await _httpClient.GetStringAsync(url);

            // Step 2: نـ parse الـ JSON
            var results = JsonSerializer.Deserialize<List<NominatimResult>>(response);

            if (results == null || results.Count == 0)
                return null;

            var result = results[0];

            var lat = double.Parse(result.lat, System.Globalization.CultureInfo.InvariantCulture);
            var lng = double.Parse(result.lon, System.Globalization.CultureInfo.InvariantCulture);

            // Step 3: نرجع النتيجة
            return new GeocodingResult
            {
                Latitude = lat,
                Longitude = lng,
                DisplayName = result.display_name,
                MapLink = GetMapLink(lat, lng)
            };
        }

        public string GetMapLink(double lat, double lng)
        {
            var latStr = lat.ToString(System.Globalization.CultureInfo.InvariantCulture);
            var lngStr = lng.ToString(System.Globalization.CultureInfo.InvariantCulture);
            return $"https://www.google.com/maps/@{latStr},{lngStr},15z";
        }

        public double CalculateDistance(double lat1, double lng1, double lat2, double lng2)
        {
            // Haversine Formula — بتحسب المسافة بين نقطتين على الكرة الأرضية
            const double R = 6371; // نصف قطر الأرض بالكيلومتر

            var dLat = ToRad(lat2 - lat1);
            var dLng = ToRad(lng2 - lng1);

            var a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
                    Math.Cos(ToRad(lat1)) * Math.Cos(ToRad(lat2)) *
                    Math.Sin(dLng / 2) * Math.Sin(dLng / 2);

            var c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));

            return R * c; // المسافة بالكيلومتر
        }

        private double ToRad(double degrees) => degrees * Math.PI / 180;
    }

    // عشان نـ deserialize الـ JSON الجاي من Nominatim
    public class NominatimResult
    {
        public string lat { get; set; } = string.Empty;
        public string lon { get; set; } = string.Empty;
        public string display_name { get; set; } = string.Empty;
    }
}