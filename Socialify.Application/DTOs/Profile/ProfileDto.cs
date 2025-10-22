using Socialify.Application.DTOs.Post;
using Socialify.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Socialify.Application.DTOs.Profile
{
    public class ProfileDto
    {
        public string? Id { get; set; }
        public string? FullName { get; set; }
        public string? ProfilePicUrl { get; set; }
        public string? Bio { get; set; }
        public bool IsCurrentUser { get; set; }
        public RelationshipStatus Status { get; set; }
        public DateTime JoinedOn { get; set; }
    }
}
