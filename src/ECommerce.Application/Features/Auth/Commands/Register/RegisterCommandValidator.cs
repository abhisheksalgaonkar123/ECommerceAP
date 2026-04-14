using FluentValidation;

namespace ECommerce.Application.Features.Auth.Commands.Register;

public class RegisterCommandValidator:AbstractValidator<RegisterCommand>
{
    public RegisterCommandValidator()
    {
        RuleFor(c => c.FirstName).NotEmpty().MaximumLength(100);
        RuleFor(c => c.LastName).NotEmpty().MaximumLength(100);;
        RuleFor(c => c.Email).NotEmpty().EmailAddress();
        RuleFor(c => c.Password).NotEmpty().MinimumLength(8);
        RuleFor(c => c.ConfirmPassword).NotEmpty().Equal(x=>x.Password);

    }
}
