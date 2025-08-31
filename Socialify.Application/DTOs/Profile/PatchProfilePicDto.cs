using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Socialify.Application.Validation;
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
        [FromForm]
        [ImageValidationAttribute]
        [Display(Name ="Profile Picture")]
        public IFormFile ProfilePicUrl { get; set; }
    }
}
