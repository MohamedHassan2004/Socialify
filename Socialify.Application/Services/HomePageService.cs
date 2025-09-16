using Socialify.Application.DTOs.Home;
using Socialify.Application.Interfaces;
using Socialify.Application.Services_Interfaces;
using Socialify.Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Socialify.Application.Services
{
    public class HomePageService : IHomePageService
    {
        private readonly IProfileService _profileService;
        //private readonly IPostService _postService;
        //private readonly IFriendService _friendService;

        public HomePageService(IProfileService profileService)//, IPostService postService, IFriendService friendService)
        {
            _profileService = profileService;
            //_postService = postService;
            //_friendService = friendService;
        }

        public async Task<Result<HomePageDto>> GetHomePageAsync(string currentUserId)
        {
            var userInfo = await _profileService.GetProfileBasicInfoAsync(currentUserId);
            //var posts = await _postService.GetFeedPostsAsync(currentUserId);
            //var peopleYouMayKnow = await _friendService.GetPeopleYouMayKnowAsync(currentUserId);

            var dto = new HomePageDto
            {
                User = userInfo.Data,
                //Posts = posts.Data ?? new List<PostDto>(),
                //PostForm = new PostFormDto { UserId = currentUserId },
                //PeopleYouMayKnow = peopleYouMayKnow.Data ?? new List<UserDto>()
            };

            return Result<HomePageDto>.Success(dto);
        }
    }

}
