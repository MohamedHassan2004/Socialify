using Microsoft.Extensions.Logging;
using Socialify.Application.DTOs.Search;
using Socialify.Application.Interfaces;
using Socialify.Application.Services_Interfaces;
using Socialify.Domain.Common;

namespace Socialify.Application.Services
{
    public class SearchService : ISearchService
    {
        private readonly IProfileService _profileService;
        private readonly IPostService _postService;
        private readonly ILogger<SearchService> _logger;

        public SearchService(IProfileService profileService, IPostService postService, ILogger<SearchService> logger)
        {
            _profileService = profileService;
            _postService = postService;
            _logger = logger;
        }

        public async Task<Result<SearchDto>> SearchAsync(string keyword, int pageNumber, int pageSize, string currentUserId)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(keyword))
                {
                    return Result<SearchDto>.Failure("The keyword is empty!");
                }

                keyword = keyword.Trim().ToLower();

                var profilesResult = await _profileService.SearchProfilesAsync(keyword, pageNumber, pageSize, currentUserId);
                var postsResult = await _postService.SearchPostsAsync(keyword, pageNumber, pageSize, currentUserId);

                if (!profilesResult.IsSuccess || !postsResult.IsSuccess)
                {
                    return Result<SearchDto>.Failure("An error occurred during search.");
                }

                var dto = new SearchDto
                {
                    Profiles = profilesResult.Data,
                    Posts = postsResult.Data
                };

                return Result<SearchDto>.Success(dto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while searching with keyword: {Keyword}", keyword);
                return Result<SearchDto>.Failure($"Error while searching");
            }
        }
    }
}
