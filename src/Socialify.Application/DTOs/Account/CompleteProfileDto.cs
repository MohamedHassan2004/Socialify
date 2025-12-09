using Socialify.Application.Validation;
using System;
using System.ComponentModel.DataAnnotations;

namespace Socialify.Application.DTOs.Account
{
    public class CompleteProfileDto
    {
        [Required]
        [RegularExpression(@"^\d{11}$", ErrorMessage = "{0} must be exactly 11 characters long.")]
        [DataType(DataType.PhoneNumber)]
        [Display(Name = "Phone Number")]
        public string? PhoneNumber { get; set; }
        [Required]
        public int Gender { get; set; }
        [MaxLength(250, ErrorMessage = "{0} cannot exceed {1} characters.")]
        public string? Bio { get; set; }

        [Required]
        [DataType(DataType.Date)]
        [Display(Name = "Birth Date")]
        [ValidateAge]
        public DateTime BirthDate { get; set; }
    }
}
