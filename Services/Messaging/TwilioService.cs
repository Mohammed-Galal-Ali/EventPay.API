using Twilio;
using Twilio.Rest.Api.V2010.Account;

namespace EventPay.API.Services.Messaging
{
    public class TwilioService
    {
        private readonly IConfiguration _config;
        public TwilioService(IConfiguration config)
        {
            _config = config;
            TwilioClient.Init(
                _config["Twilio:AccountSid"],
                _config["Twilio:AuthToken"]
            );

        }
        public async Task SendSmsAsync(string to, string message)
        {

            await MessageResource.CreateAsync(
                body: message,
                from: new Twilio.Types.PhoneNumber(_config["Twilio:FromNumber"]),
                to: new Twilio.Types.PhoneNumber(to)
            );
        }
        public async Task SendWhatsAppAsync(string to, string message)
        {
            await MessageResource.CreateAsync(
                body: message,
                from: new Twilio.Types.PhoneNumber($"whatsapp:{_config["Twilio:WhatsAppNumber"]}"),
                to: new Twilio.Types.PhoneNumber($"whatsapp:{to}")
            );
        }
    }
}
