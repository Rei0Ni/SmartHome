using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentValidation;
using SmartHome.Dto.Device;

namespace SmartHome.Application.Validations.Device
{
    public class GetDeviceDtoValidator : AbstractValidator<GetDeviceDto>
    {
        public GetDeviceDtoValidator()
        {
            RuleFor(x => x.Id)
                .NotEmpty().WithMessage("Id is required.");
        }
    }
}
