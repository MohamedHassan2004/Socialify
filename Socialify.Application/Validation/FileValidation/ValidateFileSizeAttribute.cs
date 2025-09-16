using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Socialify.Application.Validation.ImgValidation
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false)]
    public sealed class ValidateFileSizeAttribute : ValidationAttribute
    {
        private const int DefaultMaxFileSizeInBytes = 5 * 1024 * 1024;

        public int MaxFileSizeInBytes { get; }

        public ValidateFileSizeAttribute(int maxFileSizeInBytes = DefaultMaxFileSizeInBytes)
        {
            MaxFileSizeInBytes = maxFileSizeInBytes;
            ErrorMessage = $"File size exceeds the maximum limit of {MaxFileSizeInBytes / (1024 * 1024)} MB.";
        }

        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {

            if (value is not IFormFile file)
            {
                return new ValidationResult("Invalid data.");
            }

            if (file.Length > MaxFileSizeInBytes)
            {
                return new ValidationResult(ErrorMessage);
            }

            return ValidationResult.Success;
        }
    }
}
