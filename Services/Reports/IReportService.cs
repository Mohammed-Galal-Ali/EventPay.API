using EventPay.API.Models;

namespace EventPay.API.Services.Reports
{
    public interface IReportService
    {
        Task<object> GetTicketsAsync(int page, int pageSize, string? status);
        Task<byte[]> GenerateTicketsExcelAsync();
        Task<byte[]> GenerateTicketsPdfAsync();
        Task<object> GetAnalyticsAsync();
    }
}
