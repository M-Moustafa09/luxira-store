using FluentValidation;
using Luxira.Application.DTOs.Auth;

namespace Luxira.Application.Validators.Auth;

public class LogoutRequestValidator : AbstractValidator<LogoutRequest>
{
    public LogoutRequestValidator()
    {
        RuleFor(x => x.RefreshToken).NotEmpty();
    }
}
