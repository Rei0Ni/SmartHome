﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentValidation;
using SmartHome.Dto.Controller;

namespace SmartHome.Application.Validations.Controller
{
    public class UpdateControllerDtoValidator : AbstractValidator<UpdateControllerDto>
    {
        public UpdateControllerDtoValidator()
        {
            RuleFor(x => x.Id)
                .NotEmpty().WithMessage("Id is required.");
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Name is required.")
                .MaximumLength(50).WithMessage("Name must not exceed 50 characters.");
        }
    }
}
