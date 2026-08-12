using FluentValidation;
using Luxira.Application.DTOs.Auth;

namespace Luxira.Application.Validators.Auth;

public class RefreshRequestValidator : AbstractValidator<RefreshRequest>
{
    public RefreshRequestValidator()
    {
        RuleFor(x => x.RefreshToken).NotEmpty();
    }
}
