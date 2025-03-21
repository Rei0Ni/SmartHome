using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace SmartHome.Dto.Annotations
{
    public class TotpValidationAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value == null || string.IsNullOrWhiteSpace(value.ToString()))
            {
                return new ValidationResult("TOTP is required.");
            }

            string totp = value.ToString();

            if (!Regex.IsMatch(totp, @"^\d{6}$"))
            {
                return new ValidationResult("TOTP must be a 6-digit number.");
            }

            return ValidationResult.Success;
        }
    }
}
