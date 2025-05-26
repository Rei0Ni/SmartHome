using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentValidation;
using SmartHome.Dto.Device;

namespace SmartHome.Application.Validations.Device
{
    public class CreateDeviceDtoValidator : AbstractValidator<CreateDeviceDto>
    {
        public CreateDeviceDtoValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Name is required.")
                .MaximumLength(50).WithMessage("Name must not exceed 50 characters.");
            RuleFor(x => x.Description)
                .MaximumLength(100).WithMessage("Description must not exceed 100 characters.");
            RuleFor(x => x.Manufacturer)
                .MaximumLength(50).WithMessage("Manufacturer must not exceed 50 characters.");
            RuleFor(x => x.Brand)
                .MaximumLength(50).WithMessage("Brand must not exceed 50 characters.");
            RuleFor(x => x.Model)
                .MaximumLength(50).WithMessage("Model must not exceed 50 characters.");
            RuleFor(x => x.SerialNumber)
                .MaximumLength(50).WithMessage("SerialNumber must not exceed 50 characters.");
            RuleFor(x => x.AreaId)
                .NotEmpty().WithMessage("AreaId is required.");
            RuleFor(x => x.DeviceTypeId)
                .NotEmpty().WithMessage("DeviceType is required.");
            RuleFor(x => x.Pin)
                .NotEmpty().WithMessage("Pin is required.");
        }
    }
}
