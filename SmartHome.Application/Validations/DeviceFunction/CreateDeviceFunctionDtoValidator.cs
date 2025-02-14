using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentValidation;
using SmartHome.Dto.DeviceFunction;

namespace SmartHome.Application.Validations.DeviceFunction
{
    public class CreateDeviceFunctionDtoValidator : AbstractValidator<CreateDeviceFunctionDto>
    {
        public CreateDeviceFunctionDtoValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Function Name is required.");
            RuleFor(x => x.Parameters)
                .NotEmpty().WithMessage("Parameters is required and cannot be empty.");
        }
    }
}
