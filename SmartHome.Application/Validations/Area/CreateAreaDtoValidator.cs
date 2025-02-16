using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentValidation;
using SmartHome.Dto.Area;

namespace SmartHome.Application.Validations.Area
{
    public class CreateAreaDtoValidator : AbstractValidator<CreateAreaDto>
    {
        public CreateAreaDtoValidator()
        {
            RuleFor(x => x.ControllerId)
                .NotEmpty().WithMessage("ControllerId is required.");
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Name is required.")
                .MaximumLength(50).WithMessage("Name must not exceed 50 characters.");
        }
    }
}
