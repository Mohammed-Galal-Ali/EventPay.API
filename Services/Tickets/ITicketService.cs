namespace EventPay.API.Services.Tickets
{
    public interface ITicketService
    {
        Task<IEnumerable<object>> GetTicketsByEmailAsync(string email);

    }
}
