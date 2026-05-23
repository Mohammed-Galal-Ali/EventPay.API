using ClosedXML.Excel;
using EventPay.API.Data;
using EventPay.API.Models;
using Microsoft.EntityFrameworkCore;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace EventPay.API.Services.Reports
{
    public class ReportService : IReportService
    {
        private readonly AppDbContext _context;

        public ReportService(AppDbContext context)
        {
            _context = context;
        }

        
        public async Task<byte[]> GenerateTicketsExcelAsync()
        {
            var tickets = await _context.Tickets
                .Include(t => t.Event)
                .ToListAsync();

            using var workbook = new XLWorkbook();
            var worksheet = workbook.Worksheets.Add("التذاكر");

            worksheet.Cell(1, 1).Value = "رقم التذكرة";
            worksheet.Cell(1, 2).Value = "اسم المشتري";
            worksheet.Cell(1, 3).Value = "الإيميل";
            worksheet.Cell(1, 4).Value = "التليفون";
            worksheet.Cell(1, 5).Value = "الفعالية";
            worksheet.Cell(1, 6).Value = "المبلغ";
            worksheet.Cell(1, 7).Value = "الحالة";
            worksheet.Cell(1, 8).Value = "التاريخ";

            var headerRow = worksheet.Row(1);
            headerRow.Style.Font.Bold = true;
            headerRow.Style.Fill.BackgroundColor = XLColor.FromHtml("#6c63ff");
            headerRow.Style.Font.FontColor = XLColor.White;

            for (int i = 0; i < tickets.Count; i++)
            {
                var ticket = tickets[i];
                var row = i + 2;

                worksheet.Cell(row, 1).Value = ticket.Id;
                worksheet.Cell(row, 2).Value = ticket.BuyerName;
                worksheet.Cell(row, 3).Value = ticket.BuyerEmail;
                worksheet.Cell(row, 4).Value = ticket.BuyerPhone;
                worksheet.Cell(row, 5).Value = ticket.Event?.Title ?? "—";
                worksheet.Cell(row, 6).Value = ticket.PricePaid;
                worksheet.Cell(row, 7).Value = ticket.Status.ToString();
                worksheet.Cell(row, 8).Value = ticket.CreatedAt.ToString("yyyy-MM-dd");

                if (i % 2 == 0)
                    worksheet.Row(row).Style.Fill.BackgroundColor = XLColor.FromHtml("#f5f5ff");
            }

            worksheet.Columns().AdjustToContents();

            using var stream = new MemoryStream();
            workbook.SaveAs(stream);
            return stream.ToArray();
        }

        public async Task<byte[]> GenerateTicketsPdfAsync()
        {
            var tickets = await _context.Tickets
                .Include(t => t.Event)
                .ToListAsync();

            QuestPDF.Settings.License = LicenseType.Community;

            var pdf = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4);
                    page.Margin(30);
                    page.DefaultTextStyle(x => x.FontSize(11));

                    page.Header().Column(col =>
                    {
                        col.Item().Text("EventPay — تقرير التذاكر")
                            .FontSize(20).Bold()
                            .FontColor(Color.FromHex("6c63ff"));

                        col.Item().Text($"تاريخ التقرير: {DateTime.Now:yyyy-MM-dd}")
                            .FontSize(10).FontColor(Color.FromHex("888888"));

                        col.Item().PaddingTop(5).LineHorizontal(1)
                            .LineColor(Color.FromHex("6c63ff"));
                    });

                    page.Content().PaddingTop(20).Table(table =>
                    {
                        table.ColumnsDefinition(columns =>
                        {
                            columns.ConstantColumn(40);
                            columns.RelativeColumn(2);
                            columns.RelativeColumn(3);
                            columns.RelativeColumn(2);
                            columns.ConstantColumn(60);
                            columns.ConstantColumn(60);
                        });

                        table.Header(header =>
                        {
                            header.Cell().Background(Color.FromHex("6c63ff"))
                                .Padding(5).Text("#").FontColor(Colors.White).Bold();
                            header.Cell().Background(Color.FromHex("6c63ff"))
                                .Padding(5).Text("الاسم").FontColor(Colors.White).Bold();
                            header.Cell().Background(Color.FromHex("6c63ff"))
                                .Padding(5).Text("الإيميل").FontColor(Colors.White).Bold();
                            header.Cell().Background(Color.FromHex("6c63ff"))
                                .Padding(5).Text("الفعالية").FontColor(Colors.White).Bold();
                            header.Cell().Background(Color.FromHex("6c63ff"))
                                .Padding(5).Text("المبلغ").FontColor(Colors.White).Bold();
                            header.Cell().Background(Color.FromHex("6c63ff"))
                                .Padding(5).Text("الحالة").FontColor(Colors.White).Bold();
                        });

                        for (int i = 0; i < tickets.Count; i++)
                        {
                            var ticket = tickets[i];
                            var bgColor = i % 2 == 0
                                ? Color.FromHex("f5f5ff")
                                : Colors.White;

                            table.Cell().Background(bgColor).Padding(5)
                                .Text(ticket.Id.ToString());
                            table.Cell().Background(bgColor).Padding(5)
                                .Text(ticket.BuyerName);
                            table.Cell().Background(bgColor).Padding(5)
                                .Text(ticket.BuyerEmail);
                            table.Cell().Background(bgColor).Padding(5)
                                .Text(ticket.Event?.Title ?? "—");
                            table.Cell().Background(bgColor).Padding(5)
                                .Text($"{ticket.PricePaid} ج");
                            table.Cell().Background(bgColor).Padding(5)
                                .Text(ticket.Status.ToString());
                        }
                    });

                    page.Footer().AlignCenter()
                        .Text($"إجمالي التذاكر: {tickets.Count} | إجمالي المبيعات: {tickets.Sum(t => t.PricePaid)} جنيه")
                        .FontSize(10).FontColor(Color.FromHex("888888"));
                });
            });

            return pdf.GeneratePdf();
        }

        public async Task<object> GetTicketsAsync(int page, int pageSize, string? status)
        {
            var query = _context.Tickets
                .Include(t => t.Event)
                .AsQueryable();

            // Filter
            if (!string.IsNullOrEmpty(status) && status != "All")
            {
                if (Enum.TryParse<TicketStatus>(status, out var ticketStatus))
                    query = query.Where(t => t.Status == ticketStatus);
            }

            // Total count قبل الـ pagination
            var totalCount = await query.CountAsync();

            // Pagination
            var tickets = await query
                .OrderByDescending(t => t.CreatedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(t => new
                {
                    t.Id,
                    t.BuyerName,
                    t.BuyerEmail,
                    t.BuyerPhone,
                    EventTitle = t.Event.Title,
                    t.PricePaid,
                    Status = t.Status.ToString(),
                    t.CreatedAt
                })
                .ToListAsync();

            return new
            {
                data = tickets,
                totalCount,
                page,
                pageSize,
                totalPages = (int)Math.Ceiling((double)totalCount / pageSize)
            };
        }
    }
}