using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using FluentValidation;
using SmartHome.Dto.Area;

namespace SmartHome.Application.Validations.Area
{
    public class GetAreaDtoValidator : AbstractValidator<GetAreaDto>
    {
        public GetAreaDtoValidator()
        {
            RuleFor(x => x.Id)
                .NotEmpty().WithMessage("Id is required.");
        }
    }
}
