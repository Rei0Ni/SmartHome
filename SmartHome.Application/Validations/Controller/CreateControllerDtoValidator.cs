using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentValidation;
using SmartHome.Dto.Controller;

namespace SmartHome.Application.Validations.Controller
{
    public class CreateControllerDtoValidator : AbstractValidator<CreateControllerDto>
    {
        public CreateControllerDtoValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Name is required.")
                .MaximumLength(50).WithMessage("Name must not exceed 50 characters.");

            RuleFor(x => x.MACAddress)
                .NotEmpty().WithMessage("MAC Address is required.")
                .Matches(@"^([0-9A-Fa-f]{2}[:-]){5}([0-9A-Fa-f]{2})$").WithMessage("Invalid MAC Address.");

            RuleFor(x => x.IPAddress)
                .NotEmpty().WithMessage("IP Address is required.")
                .Matches(@"\b\d{1,3}\.\d{1,3}\.\d{1,3}\.\d{1,3}\b").WithMessage("Invalid IP Address.");
        }
    }
}
