using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentValidation;
using SmartHome.Dto.User;

namespace SmartHome.Application.Validations.User
{
    public class UpdatePasswordDtoValidator : AbstractValidator<UpdatePasswordDto>
    {
        public UpdatePasswordDtoValidator()
        {
            RuleFor(x => x.Username).NotEmpty().WithMessage("Username is required.");
            RuleFor(x => x.NewPassword).NotEmpty().WithMessage("New password is required.");
            RuleFor(x => x.RetypePassword).NotEmpty().Equal(x => x.NewPassword).WithMessage("Password Mismatch");
            RuleFor(x => x.Totp).NotEmpty().WithMessage("TOTP Is Required");
        }
    }
}
