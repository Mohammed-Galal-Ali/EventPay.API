namespace EventPay.API.DTOs.Tickets
{
    public class GetMyTicketsDto
    {
        public string Email { get; set; } = string.Empty;
        public string OtpCode { get; set; } = string.Empty;
    }
}
