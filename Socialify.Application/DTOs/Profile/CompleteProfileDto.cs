using Socialify.Application.Validation;
using System;
using System.ComponentModel.DataAnnotations;

namespace Socialify.Application.DTOs.Profile
{
    public class CompleteProfileDto
    {
        [Required]
        [Length(11, 11, ErrorMessage = "Phone number must be exactly 11 characters long.")]
        [DataType(DataType.PhoneNumber)]
        [Display(Name = "Phone Number")]
        public string? PhoneNumber { get; set; }
        [Required]
        public int Gender { get; set; }
        [MaxLength(250, ErrorMessage = "Bio cannot exceed 250 characters.")]
        public string? Bio { get; set; }

        [Required]
        [DataType(DataType.Date)]
        [Display(Name = "Birth Date")]
        [AgeValidation]
        public DateTime BirthDate { get; set; }
    }
}
