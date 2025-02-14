using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentValidation;
using SmartHome.Dto.Controller;

namespace SmartHome.Application.Validations.Controller
{
    public class GetControllerDtoValidator : AbstractValidator<GetControllerDto>
    {
        public GetControllerDtoValidator()
        {
            RuleFor(x => x.Id)
                .NotEmpty().WithMessage("Id is required.");
        }
    }
}
