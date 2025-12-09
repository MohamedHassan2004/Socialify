using Socialify.Application.DTOs.Common;
using Socialify.Application.DTOs.Post;
using Socialify.Application.DTOs.Profile;
using Socialify.Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Socialify.Application.DTOs.Home
{
    public class HomePageDto
    {
        public ProfileBasicInfoDto User { get; set; }
        public PagedResult<PostDto> Posts { get; set; } = new();
        //public PagedResult<ProfileBasicInfoDto> People { get; set; } = new();
    }
}
