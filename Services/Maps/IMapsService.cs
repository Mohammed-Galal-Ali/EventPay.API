namespace EventPay.API.Services.Maps
{
    public interface IMapsService
    {
        Task<GeocodingResult?> GeocodeAsync(string address);
        string GetMapLink(double lat, double lng);
        double CalculateDistance(double lat1, double lng1, double lat2, double lng2);
    }

    public class GeocodingResult
    {
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public string DisplayName { get; set; } = string.Empty;
        public string MapLink { get; set; } = string.Empty;
    }
}