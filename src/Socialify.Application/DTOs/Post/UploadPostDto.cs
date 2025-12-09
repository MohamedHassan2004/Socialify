using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
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
    public class UploadPostDto
    {
        [StringLength(500, ErrorMessage = "Content length can't be more than 500.")]
        public string? Content { get; set; }
        [FromForm]
        [ValidateFileSize(5 * 1024 * 1024)]
        [ValidateFileType(FileType.Media)]
        public IFormFile? MediaFile { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
