using Socialify.Application.DTOs.Search;
using Socialify.Domain.Common;
using System.Threading.Tasks;

namespace Socialify.Application.Services_Interfaces
{
    public interface ISearchService
    {
        Task<Result<SearchDto>> SearchAsync(string keyword, int page, int pageSize, string currentUserId);
    }
}