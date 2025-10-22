using Socialify.Application.DTOs.Common;
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
        public ProfileDto ProfileInfo { get; set; } = new();
        public PagedResult<PostDto> Posts { get; set; } = new();
        public PagedResult<ProfileBasicInfoDto> Friends { get; set; } = new();
    }
}
