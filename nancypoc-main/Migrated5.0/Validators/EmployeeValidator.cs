using FluentValidation;
using Employee.Contracts;

namespace Employee.Host.Validators
{
    public class EmployeeValidator : AbstractValidator<EmployeeRequest>
    {
        public EmployeeValidator()
        {
            RuleFor(x => x.Username)
                .NotEmpty()
                .WithMessage("Username is required");

n            RuleFor(x => x.Email)
                .NotEmpty()
                .WithMessage("Email is required")
                .EmailAddress()
                .WithMessage("Email must be a valid email address");

n            RuleFor(x => x.Role)
                .NotEmpty()
                .WithMessage("Role is required");

n            When(x => x.EmpId == 0, () =>
            {
                RuleFor(x => x.PasswordHash)
                    .NotEmpty()
                    .WithMessage("Password is required when creating employee")
                    .MinimumLength(6)
                    .WithMessage("Password must be at least 6 characters long");
            });
        }
    }
}
