using Microsoft.AspNetCore.Http;
using Socialify.Application.Validation.FileValidation;
using Socialify.Application.Validation.ImgValidation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Socialify.Application.DTOs.Post
{
    public class UpdatePostDto
    {
        public int Id { get; set; }
        [StringLength(1000, ErrorMessage = "Content length can't be more than 1000.")]
        public string? Content { get; set; }
        [ValidateFileSize(5 * 1024 * 1024)]
        [ValidateFileType(FileType.Media)]
        public IFormFile? MediaFile { get; set; }
        public string? MediaUrl { get; set; }
        public string? MediaType { get; set; }
        public bool RemoveMedia { get; set; }
        public string UserId { get; set; } = string.Empty;
    }
}
