using FluentValidation;
using EventPay.API.DTOs.Payments;

namespace EventPay.API.Validators
{
    public class CreatePaymentValidator : AbstractValidator<CreatePaymentDto>
    {
        public CreatePaymentValidator()
        {
            RuleFor(x => x.EventId)
                .GreaterThan(0).WithMessage("Invalid event");

            RuleFor(x => x.BuyerName)
                .NotEmpty().WithMessage("Name is required")
                .MinimumLength(3).WithMessage("Name must be at least 3 characters");

            RuleFor(x => x.BuyerEmail)
                .NotEmpty().WithMessage("Email is required")
                .EmailAddress().WithMessage("Invalid email address");

            RuleFor(x => x.BuyerPhone)
                .NotEmpty().WithMessage("Phone number is required")
                .Must(BeValidEgyptianPhone)
                .WithMessage("Phone must start with 01 and be 11 digits (e.g. 01009633234)");
        }

        private bool BeValidEgyptianPhone(string phone)
        {
            if (string.IsNullOrEmpty(phone)) return false;

            phone = phone.Trim();

            if (!phone.StartsWith("01")) return false;
            if (phone.Length != 11) return false;
            if (!phone.All(char.IsDigit)) return false;

            var prefix = phone.Substring(0, 3);
            var validPrefixes = new[] { "010", "011", "012", "015" };
            return validPrefixes.Contains(prefix);
        }
    }
}