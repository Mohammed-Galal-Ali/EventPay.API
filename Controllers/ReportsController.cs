using EventPay.API.Services.Reports;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles ="Admin")]
public class ReportsController : ControllerBase
{
    private readonly IReportService _reportService;

    public ReportsController(IReportService reportService)
    {
        _reportService = reportService;
    }
    [HttpGet("tickets")]
    public async Task<IActionResult> GetTickets(
    [FromQuery] int page = 1,
    [FromQuery] int pageSize = 10,
    [FromQuery] string? status = null)
    {
        var result = await _reportService.GetTicketsAsync(page, pageSize, status);
        return Ok(result);
    }

    [HttpGet("tickets/excel")]
    public async Task<IActionResult> ExportTicketsExcel()
    {
        var bytes = await _reportService.GenerateTicketsExcelAsync();
        return File(bytes,
            "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
            "tickets.xlsx");
    }

    [HttpGet("tickets/pdf")]
    public async Task<IActionResult> ExportTicketsPdf()
    {
        var bytes = await _reportService.GenerateTicketsPdfAsync();
        return File(bytes, "application/pdf", "tickets.pdf");
    }

    [HttpGet("analytics")]
    public async Task<IActionResult> GetAnalytics()
    {
        var analytics = await _reportService.GetAnalyticsAsync();
        return Ok(analytics);
    }
}