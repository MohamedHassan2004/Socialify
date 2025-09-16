using AutoMapper;
using Socialify.Application.DTOs.Profile;
using Socialify.Application.DTOs.Search;
using Socialify.Application.ReposInterfaces;
using Socialify.Application.Services_Interfaces;
using Socialify.Domain.Common;

public class SearchService : ISearchService
{
    private readonly ISearchRepository _searchRepository;
    private readonly IMapper _mapper;

    public SearchService(ISearchRepository searchRepository, IMapper mapper)
    {
        _searchRepository = searchRepository;
        _mapper = mapper;
    }

    public async Task<Result<SearchResultDto>> SearchAsync(string keyword)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(keyword))
            {
                return Result<SearchResultDto>.Failure("The keyword is empty!");
            }

            keyword = keyword.Trim().ToLower();

            var users = await _searchRepository.SearchUsersAsync(keyword);

            var dto = new SearchResultDto
            {
                Users = _mapper.Map<IEnumerable<ProfileBasicInfoDto>>(users)
            };

            return Result<SearchResultDto>.Success(dto);
        }
        catch (Exception ex)
        {
            return Result<SearchResultDto>.Failure($"Error while searching: {ex.Message}");
        }
    }
}