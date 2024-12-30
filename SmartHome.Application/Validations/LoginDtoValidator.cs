using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentValidation;
using SmartHome.Application.DTOs.User;

namespace SmartHome.Application.Validations
{
    public class LoginDtoValidator : AbstractValidator<LoginDto>
    {
        public LoginDtoValidator()
        {
            RuleFor(x => x.Username).NotEmpty().WithMessage("Your username cannot be empty");
            //.MinimumLength(8).WithMessage("Your username length must be at least 8.");

            RuleFor(x => x.Password).NotEmpty().WithMessage("Your password cannot be empty");
                //.MinimumLength(8).WithMessage("Your password length must be at least 8.")
                //.MaximumLength(16).WithMessage("Your password length must not exceed 16.")
                //.Matches(@"[A-Z]+").WithMessage("Your password must contain at least one uppercase letter.")
                //.Matches(@"[a-z]+").WithMessage("Your password must contain at least one lowercase letter.")
                //.Matches(@"[0-9]+").WithMessage("Your password must contain at least one number.")
                //.Matches(@"[\!\?\*\.\@\#]+").WithMessage("Your password must contain at least one (!? *.).");
        }
    }
}
