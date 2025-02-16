using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentValidation;
using SmartHome.Dto.DeviceFunction;

namespace SmartHome.Application.Validations.DeviceFunction
{
    public class DeleteDeviceFunctionDtoValidator : AbstractValidator<DeleteDeviceFunctionDto>
    {
        public DeleteDeviceFunctionDtoValidator()
        {
            RuleFor(x => x.Id)
                .NotEmpty().WithMessage("Id is required.");
        }
    }
}
