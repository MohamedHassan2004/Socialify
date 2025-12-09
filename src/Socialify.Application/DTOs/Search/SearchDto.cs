using Socialify.Application.DTOs.Common;
using Socialify.Application.DTOs.Post;
using Socialify.Application.DTOs.Profile;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Socialify.Application.DTOs.Search
{
    public class SearchDto
    {
        public PagedResult<ProfileBasicInfoDto> Profiles { get; set; } = new();
        public PagedResult<PostDto> Posts { get; set; } = new();
    }
}
