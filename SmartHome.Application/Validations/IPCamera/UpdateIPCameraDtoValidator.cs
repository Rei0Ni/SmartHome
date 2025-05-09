using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentValidation;
using SmartHome.Dto.IPCamera;

namespace SmartHome.Application.Validations.IPCamera
{
    public class UpdateIPCameraDtoValidator : AbstractValidator<UpdateIPCameraDto>
    {
        public UpdateIPCameraDtoValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Name is required.").MaximumLength(50)
                .WithMessage("Name must not exceed 50 characters.");
            RuleFor(x => x.IPAddress)
                .NotEmpty().WithMessage("IP Address is required.")
                .Matches(@"\b\d{1,3}\.\d{1,3}\.\d{1,3}\.\d{1,3}\b").WithMessage("Invalid IP Address.");
            RuleFor(x => x.Username)
                .NotEmpty().WithMessage("Username is required.")
                .MaximumLength(50).WithMessage("Username must not exceed 50 characters.");
            RuleFor(x => x.Password)
                .NotEmpty().WithMessage("Password is required.")
                .MaximumLength(50).WithMessage("Password must not exceed 50 characters.");
            RuleFor(x => x.AreaId)
                .NotEmpty().WithMessage("Area ID is required.")
                .Must(x => Guid.TryParse(x.ToString(), out _)).WithMessage("Invalid Area ID.");
        }
    }
}
