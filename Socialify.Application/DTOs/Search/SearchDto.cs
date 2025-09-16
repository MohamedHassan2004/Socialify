using Socialify.Application.DTOs.Post;
using Socialify.Application.DTOs.Profile;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Socialify.Application.DTOs.Search
{
    public class SearchResultDto
    {
        public IEnumerable<ProfileBasicInfoDto> Users { get; set; }
        public IEnumerable<PostDto> Posts { get; set; }
    }
}
