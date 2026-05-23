using EventPay.API.DTOs.Events;
using FluentValidation;

namespace EventPay.API.Validators
{
    public class CreateEventValidator:AbstractValidator<CreateEventDto>
    {
        public CreateEventValidator()
        {
            RuleFor(x => x.Title)
              .NotEmpty().WithMessage("Title is required")
              .MinimumLength(3).WithMessage("Title must be at least 3 characters")
              .MaximumLength(100).WithMessage("Title must not exceed 100 characters");

            RuleFor(x => x.Description)
                .NotEmpty().WithMessage("Description is required")
                .MinimumLength(10).WithMessage("Description must be at least 10 characters");

            RuleFor(x => x.Date)
                .NotEmpty().WithMessage("Date is required")
                .GreaterThan(DateTime.UtcNow).WithMessage("Event date must be in the future");

            RuleFor(x => x.Location)
                .NotEmpty().WithMessage("Location is required");

            RuleFor(x => x.TicketPrice)
                .GreaterThan(0).WithMessage("Ticket price must be greater than 0");

            RuleFor(x => x.TotalTickets)
                .GreaterThan(0).WithMessage("Total tickets must be greater than 0");
        }
    }
}
