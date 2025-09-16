using Socialify.Application.DTOs.Post;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Socialify.Application.DTOs.Profile
{
    public class ProfilePageDto
    {
        public ProfileDto User { get; set; } = new ProfileDto();
        public ICollection<PostDto> Posts { get; set; } = new List<PostDto>();
        public ICollection<ProfileBasicInfoDto> Friends { get; set; } = new List<ProfileBasicInfoDto>();
    }
}
