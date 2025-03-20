using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentValidation;
using SmartHome.Dto.DeviceFunction;

namespace SmartHome.Application.Validations.DeviceFunction
{
    public class UpdateDeviceFunctionDtoValidator : AbstractValidator<UpdateDeviceFunctionDto>
    {
        public UpdateDeviceFunctionDtoValidator()
        {
            RuleFor(x => x.Id).NotEmpty().WithMessage("Id is required.");
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Name is required.")
                .MaximumLength(50).WithMessage("Name must not exceed 50 characters.");
            RuleFor(x => x.Parameters)
                .NotEmpty().WithMessage("Parameters is required and cannot be empty.");
        }
    }
}
