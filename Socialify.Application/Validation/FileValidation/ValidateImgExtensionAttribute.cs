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
        private static readonly string[] AllowedExtensions = { ".jpg", ".jpeg", ".png", ".gif" };

        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            var file = value as IFormFile;
            if (file == null)
            {
                return new ValidationResult("No file provided.");
            }


            string fileExtension = Path.GetExtension(file.FileName).ToLowerInvariant();
            if (!AllowedExtensions.Contains(fileExtension))
            {
                return new ValidationResult($"Invalid image format. Allowed formats are: {string.Join(", ", AllowedExtensions)}");
            }

            return ValidationResult.Success;
        }
    }
}
