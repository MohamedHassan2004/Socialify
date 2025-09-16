using Socialify.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Socialify.Application.DTOs.Profile
{
    public class ProfileBasicInfoDto
    {
        public string Id { get; set; }
        public string FullName { get; set; }
        public string ProfilePicUrl { get; set; }
        public RelationshipStatus RelationshipStatus { get; set; }
    }
}
