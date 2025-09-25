using FluentValidation;
using Employee.Contracts;

namespace Employee.Host.Validators
{
    public class LoginValidator : AbstractValidator<LoginRequest>
    {
        public LoginValidator()
        {
            RuleFor(x => x.EmpId)
                .GreaterThan(0)
                .WithMessage("EmpId must be greater than 0");

n            RuleFor(x => x.Password)
                .NotEmpty()
                .WithMessage("Password is required");
        }
    }
}
