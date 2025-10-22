using Microsoft.AspNetCore.Http;
using Microsoft.VisualBasic.FileIO;
using Socialify.Application.Validation.FileValidation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Socialify.Application.Validation.FileValidation
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false)]
    public class ValidateFileTypeAttribute : ValidationAttribute
    {
        private readonly string[] _allowedExtensions;
        private readonly string[] _allowedMimeTypes;

        public ValidateFileTypeAttribute(FileType fileType)
        {
            switch (fileType)
            {
                case FileType.Image:
                    _allowedExtensions = FileValidationConstants.ImageExtensions;
                    _allowedMimeTypes = FileValidationConstants.ImageMimeTypes;
                    break;

                case FileType.Video:
                    _allowedExtensions = FileValidationConstants.VideoExtensions;
                    _allowedMimeTypes = FileValidationConstants.VideoMimeTypes;
                    break;

                case FileType.Audio:
                    _allowedExtensions = FileValidationConstants.AudioExtensions;
                    _allowedMimeTypes = FileValidationConstants.AudioMimeTypes;
                    break;

                case FileType.Media:
                    _allowedExtensions = FileValidationConstants.MediaExtensions;
                    _allowedMimeTypes = FileValidationConstants.MediaMimeTypes;
                    break;

                default:
                    _allowedExtensions = Array.Empty<string>();
                    _allowedMimeTypes = Array.Empty<string>();
                    break;
            }
        }

        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            if (value == null) return ValidationResult.Success;

            if (value is not IFormFile file)
                return new ValidationResult("Invalid file data.");

            var fileExtension = Path.GetExtension(file.FileName).ToLowerInvariant();
            if (_allowedExtensions.Any() && !_allowedExtensions.Contains(fileExtension))
                return new ValidationResult($"Invalid file format. Allowed: {string.Join(", ", _allowedExtensions)}");

            if (_allowedMimeTypes.Any() &&
                !string.IsNullOrEmpty(file.ContentType) &&
                !_allowedMimeTypes.Contains(file.ContentType.ToLowerInvariant()))
                return new ValidationResult("Invalid file type detected.");

            return ValidationResult.Success;
        }
    }

}
