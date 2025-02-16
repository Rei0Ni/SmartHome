using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentValidation;
using SmartHome.Dto.Area;

namespace SmartHome.Application.Validations.Area
{
    public class DeleteAreaDtoValidator : AbstractValidator<DeleteAreaDto>
    {
        public DeleteAreaDtoValidator()
        {
            RuleFor(x => x.Id)
                .NotEmpty().WithMessage("Id is required.");
        }
    }
}
