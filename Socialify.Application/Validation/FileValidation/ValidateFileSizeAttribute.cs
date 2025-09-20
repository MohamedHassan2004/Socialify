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
        private const int DefaultMaxFileSizeInBytes = 5 * 1024 * 1024; // 5MB

        public int MaxFileSizeInBytes { get; }

        public ValidateFileSizeAttribute(int maxFileSizeInBytes = DefaultMaxFileSizeInBytes)
        {
            MaxFileSizeInBytes = maxFileSizeInBytes;
            ErrorMessage = $"File size cannot exceed {MaxFileSizeInBytes / (1024 * 1024)} MB.";
        }

        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            if (value == null)
            {
                return ValidationResult.Success; // Let Required attribute handle null values
            }

            if (value is not IFormFile file)
            {
                return new ValidationResult("Invalid file data.");
            }

            if (file.Length == 0)
            {
                return new ValidationResult("File is empty.");
            }

            if (file.Length > MaxFileSizeInBytes)
            {
                return new ValidationResult(ErrorMessage);
            }

            return ValidationResult.Success;
        }
    }
}
