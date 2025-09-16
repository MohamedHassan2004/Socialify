using Socialify.Application.DTOs.Post;
using Socialify.Application.DTOs.Profile;
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
        public List<PostDto> Posts { get; set; } = new List<PostDto>();
        //public List<ProfileBasicInfoDto> People { get; set; } = new List<ProfileBasicInfoDto>();
    }
}
