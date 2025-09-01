using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Socialify.Application.DTOs.Profile
{
    public class ProfileDto
    {
        public string Id { get; set; }
        public string Email { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string FullName { get; set; }
        public string ProfilePicUrl { get; set; }
        public string Bio { get; set; }
        public DateTime BirthDate { get; set; }
    }
}
