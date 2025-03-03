using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentValidation;
using SmartHome.Dto.DeviceType;

namespace SmartHome.Application.Validations.DeviceType
{
    public class CreateDeviceTypeDtoValidator : AbstractValidator<CreateDeviceTypeDto>
    {
        public CreateDeviceTypeDtoValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Name is required.")
                .MaximumLength(50).WithMessage("Name must not exceed 50 characters.");
        }
    }
}
