using Microsoft.AspNetCore.Http;
using Socialify.Application.DTOs.Account;
using Socialify.Application.DTOs.Common;
using Socialify.Application.DTOs.Profile;
using Socialify.Domain.Common;
using Socialify.Domain.Entities;
using System.Threading.Tasks;

namespace Socialify.Application.Interfaces
{
    public interface IProfileService
    {
        Task<Result> UpdateProfilePictureAsync(string currentUserId, PatchProfilePicDto patchProfilePicDto);
        Task<Result> RemoveProfilePictureAsync(string currentUserId);
        Task<Result<ProfileBasicInfoDto>> GetProfileBasicInfoAsync(string currentUserId);
        Task<Result<ProfileDto>> GetUserProfileAsync(string targetUserId, string currentUserId);
        Task<Result> UpdateProfileInfoAsync(string currentUserId, UpdateProfileInfoDto updateProfileInfoDto);
        Task<Result<UpdateProfileInfoDto>> GetProfileInfoAsync(string currentUserId);
        Task<Result<PagedResult<ProfileBasicInfoDto>>> SearchProfilesAsync(string query, int page, int pageSize);
    }
}
