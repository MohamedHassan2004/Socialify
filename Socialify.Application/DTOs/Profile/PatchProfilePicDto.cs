using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Socialify.Application.Validation;
using Socialify.Application.Validation.ImgValidation;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Socialify.Application.DTOs.Profile
{
    public class PatchProfilePicDto
    {
        [Required]
        [ValidateFileSize]
        [ValidateImgExtension]
        [Display(Name ="Profile Picture")]
        public IFormFile? ProfilePicture { get; set; }
    }
}
