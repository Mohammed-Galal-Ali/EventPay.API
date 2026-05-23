using EventPay.API.Data;
using EventPay.API.DTOs.Payments;
using EventPay.API.Models;
using EventPay.API.Services.Messaging;
using Microsoft.EntityFrameworkCore;
using Stripe;
namespace EventPay.API.Services.Payments
{
    public class PaymentService : IPaymentService
    {
        private readonly AppDbContext _context;
        private readonly IConfiguration _config;
        private readonly IMessageService _messageService;
        public PaymentService(AppDbContext context, IConfiguration config,IMessageService messageService)
        {
            _context = context;
            _config = config;
            _messageService = messageService;
            StripeConfiguration.ApiKey = _config["Stripe:SecretKey"];
        }
        public async Task<PaymentResponseDto> CreatePaymentAsync(CreatePaymentDto dto)
        {
            var eventItem = await _context.Events.FindAsync(dto.EventId);
            if(eventItem == null) 
                throw new Exception("Event not found") ;
            if(eventItem.SoldTickets>=eventItem.TotalTickets)
                throw new Exception("All tickets are sold out");

            // تحويل الرقم لـ International format
            if (dto.BuyerPhone.StartsWith("0"))
                dto.BuyerPhone = "+2" + dto.BuyerPhone;
            // Step 2: نعمل Ticket بـ Status = Pending
            var ticket = new Ticket
            {
                BuyerName = dto.BuyerName,
                BuyerEmail = dto.BuyerEmail,
                BuyerPhone = dto.BuyerPhone,
                EventId = dto.EventId,
                PricePaid = eventItem.TicketPrice,
                Status = TicketStatus.Pending
            };
            _context.Tickets.Add(ticket);
            await _context.SaveChangesAsync();

            // Step 3: نعمل PaymentIntent في Stripe
            var options = new PaymentIntentCreateOptions
            {
                Amount = (long)(eventItem.TicketPrice * 100),
                Currency = "usd",
                Metadata = new Dictionary<string, string>
                {
                    { "TicketId", ticket.Id.ToString() },
                    
                }
            };

            var service = new PaymentIntentService();
            var intent = await service.CreateAsync(options);


            // Step 4: نحفظ الـ PaymentIntentId في الـ Ticket
            ticket.PaymentIntentId = intent.Id;
            await _context.SaveChangesAsync();
            return new PaymentResponseDto
            {
                TicketId = ticket.Id,
                ClientSecret = intent.ClientSecret,
                Amount = eventItem.TicketPrice,
                EventTitle = eventItem.Title
            };
        }

        public async Task HandleWebhookAsync(string paylload, string stripeSignature)
        {
            // Step 1: نتأكد إن الـ Webhook جاي من Stripe فعلاً
            var webhooksecret = _config["Stripe:WebhookSecret"];
            var stripeEvent = EventUtility.ConstructEvent(paylload, stripeSignature, webhooksecret);

            // Step 2: نشوف نوع الـ Event
            if(stripeEvent.Type == "payment_intent.succeeded")
            {
                var intent = stripeEvent.Data.Object as PaymentIntent;

                // Step 3: نجيب الـ Ticket بالـ PaymentIntentId
                var ticket = await _context.Tickets
            .Include(t => t.Event)
            .FirstOrDefaultAsync(t => t.PaymentIntentId == intent.Id);

                if (ticket == null) return;
                // Step 4: نأكد التذكرة ونزود الـ SoldTickets
                ticket.Status = TicketStatus.Paid;

                var eventItem = await _context.Events.FindAsync(ticket.EventId);
                eventItem.SoldTickets += 1;

                await _context.SaveChangesAsync();
                // ✅ بعت Confirmation
                var message = $"🎉 تم تأكيد تذكرتك!\n\n" +
              $"الفعالية: {ticket.Event.Title}\n" +
              $"الاسم: {ticket.BuyerName}\n" +
              $"المبلغ: {ticket.PricePaid} جنيه\n" +
              $"رقم التذكرة: #{ticket.Id}\n" +
              $"المكان: {ticket.Event.Location}\n" +
              $"الخريطة: {ticket.Event.MapLink}";

                // Email
                await _messageService.SendEmailAsync(
                    ticket.BuyerEmail,
                    "تأكيد تذكرتك - EventPay 🎟️",
                    message);

                // WhatsApp
                await _messageService.SendWhatsAppAsync(
                    ticket.BuyerPhone,
                    message);

                // Telegram 
                var telegramChatId = "2023709846"; // Replace with your Telegram chat ID
                await _messageService.SendTelegramAsync(
                    telegramChatId,
                    message);

            }
        }
    }
}
