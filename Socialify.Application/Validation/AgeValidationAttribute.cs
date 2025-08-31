using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Socialify.Application.Validation
{
    public class AgeValidationAttribute : ValidationAttribute
    {
        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            if (value is not DateTime birthDate)
            {
                return new ValidationResult("Invalid date of birth.");
            }

            var today = DateTime.Today;
            var age = today.Year - birthDate.Year;
            if (birthDate > today.AddYears(-age)) age--;

            if (age < 10)
            {
                return new ValidationResult("Age must be at least 10 years.");
            }
            else if (age > 100)
            {
                return new ValidationResult("Your Age is over 100 years!!");
            }

            return ValidationResult.Success;
        }
    }
}
