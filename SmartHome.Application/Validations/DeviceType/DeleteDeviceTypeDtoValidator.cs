using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentValidation;
using SmartHome.Dto.DeviceType;

namespace SmartHome.Application.Validations.DeviceType
{
    public class DeleteDeviceTypeDtoValidator : AbstractValidator<DeleteDeviceTypeDto>
    {
        public DeleteDeviceTypeDtoValidator()
        {
            RuleFor(x => x.Id)
                .NotEmpty().WithMessage("Id is required.");
        }
    }
}
