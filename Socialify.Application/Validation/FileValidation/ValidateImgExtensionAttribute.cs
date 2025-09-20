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
    public class ValidateImgExtensionAttribute : ValidationAttribute
    {
        private static readonly string[] AllowedExtensions = { ".jpg", ".jpeg", ".png", ".gif", ".webp" };
        private static readonly string[] AllowedMimeTypes = { 
            "image/jpeg", "image/jpg", "image/png", "image/gif", "image/webp" 
        };

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

            if (string.IsNullOrWhiteSpace(file.FileName))
            {
                return new ValidationResult("File name is required.");
            }

            // Check file extension
            string fileExtension = Path.GetExtension(file.FileName).ToLowerInvariant();
            if (!AllowedExtensions.Contains(fileExtension))
            {
                return new ValidationResult($"Invalid file format. Allowed formats: {string.Join(", ", AllowedExtensions)}");
            }

            // Check MIME type for additional security
            if (!string.IsNullOrEmpty(file.ContentType) && !AllowedMimeTypes.Contains(file.ContentType.ToLowerInvariant()))
            {
                return new ValidationResult("Invalid file type detected.");
            }

            return ValidationResult.Success;
        }
    }
}
