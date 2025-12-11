using Microsoft.Extensions.Logging;
using Socialify.Application.DTOs.Common;
using Socialify.Application.DTOs.Profile;
using Socialify.Application.Mappers;
using Socialify.Application.Repos_Interfaces;
using Socialify.Application.Services_Interfaces;
using Socialify.Domain.Common;
using Socialify.Domain.Entities;
using Socialify.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Socialify.Application.Services
{
    public class FriendshipService : IFriendshipService
    {
        private readonly IFriendshipRepository _friendshipRepository;
        private readonly ILogger<FriendshipService> _logger;

        public FriendshipService(IFriendshipRepository friendshipRepository, ILogger<FriendshipService> logger)
        {
            _friendshipRepository = friendshipRepository;
            _logger = logger;
        }

        public async Task<Result> AddFriendshipAsync(string userId1, string userId2)
        {
            try
            {
                var friendshipExists = await _friendshipRepository.SingleOrDefaultAsync(f => f.UserId == userId1 && f.FriendId == userId2);
                if (friendshipExists != null)
                {
                    return Result.Failure("Friendship already exists.");
                }
                var friendship1 = new Friendship
                {
                    UserId = userId1,
                    FriendId = userId2
                };
                var friendship2 = new Friendship
                {
                    UserId = userId2,
                    FriendId = userId1
                };

                await _friendshipRepository.AddAsync(friendship1);
                await _friendshipRepository.AddAsync(friendship2);

                var isAdded = await _friendshipRepository.SaveChangesAsync() > 0;
                if (!isAdded)
                {
                    return Result.Failure("Failed to add friendship.");
                }
                _logger.LogInformation("User {user1} and User {user2} have become Friends", userId1, userId2);
                return Result.Success();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while adding friendship between {UserId1} and {UserId2}", userId1, userId2);
                return Result.Failure("Error occurred while adding friendship.");
            }
        }

        public async Task<Result> RemoveFriendshipAsync(string userId1, string userId2)
        {
            try
            {
                var friendshipExists = await _friendshipRepository.FindAsync(f => f.UserId == userId1 && f.FriendId == userId2 || f.FriendId == userId1 && f.UserId == userId2);
                if (friendshipExists == null)
                {
                    return Result.Failure("Friendship does not exist.");
                }
                _friendshipRepository.RemoveRange(friendshipExists);
                var isRemoved = await _friendshipRepository.SaveChangesAsync() > 0;
                if (!isRemoved)
                {
                    return Result.Failure("Failed to remove friendship.");
                }

                _logger.LogInformation("User {user1} has removed User {user2} from his/her Friends", userId1, userId2);
                return Result.Success();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while removing friendship between {UserId1} and {UserId2}", userId1, userId2);
                return Result.Failure("Error occurred while removing friendship.");
            }
        }

        public async Task<Result<PagedResult<ProfileBasicInfoDto>>> GetMyFriendshipsAsync(PaginationParamsDto paramsDto)
        {
            try
            {
                var friendships = await _friendshipRepository.GetMyFriendshipAsync(paramsDto.CurrentUserId, paramsDto.PageNumber, paramsDto.PageSize);

                if (friendships == null || !friendships.Data.Any())
                {
                    return Result<PagedResult<ProfileBasicInfoDto>>.Success(
                        new PagedResult<ProfileBasicInfoDto>
                        {
                            Data = new List<ProfileBasicInfoDto>(),
                            TotalCount = 0,
                            PageNumber = paramsDto.PageNumber,
                            PageSize = paramsDto.PageSize
                        });
                }

                var friendDtos = friendships.Data.Select(f => f.Friend.ToProfileBasicInfoDto(paramsDto.CurrentUserId, RelationshipStatus.Friend)).ToList();

                _logger.LogInformation("Fetched Friends List for user {UserId} successfully", paramsDto.CurrentUserId);
                return Result<PagedResult<ProfileBasicInfoDto>>.Success(
                    new PagedResult<ProfileBasicInfoDto>
                    {
                        Data = friendDtos,
                        TotalCount = friendships.TotalCount,
                        PageNumber = friendships.PageNumber,
                        PageSize = friendships.PageSize
                    });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while getting friendships");
                return Result<PagedResult<ProfileBasicInfoDto>>.Failure("Error occurred while getting friendships");
            }
        }
        public async Task<Result<PagedResult<ProfileBasicInfoDto>>> GetFriendshipsByUserIdAsync(string userId, PaginationParamsDto paramsDto)
        {
            try
            {
                var friendships = await _friendshipRepository.GetFriendshipAsync(
                    userId, paramsDto.CurrentUserId, paramsDto.PageNumber, paramsDto.PageSize);

                if (friendships == null || !friendships.Data.Any())
                {
                    return Result<PagedResult<ProfileBasicInfoDto>>.Success(
                        new PagedResult<ProfileBasicInfoDto>
                        {
                            Data = new List<ProfileBasicInfoDto>(),
                            TotalCount = 0,
                            PageNumber = paramsDto.PageNumber,
                            PageSize = paramsDto.PageSize
                        });
                }

                // Direct mapping - no second query needed!
                var friendDtos = friendships.Data.Select(f => new ProfileBasicInfoDto
                {
                    Id = f.Id,
                    FullName = f.FullName,
                    ProfilePicUrl = f.ProfilePicUrl,
                    Bio = f.Bio,
                    RelationshipStatus = f.RelationshipStatus
                }).ToList();

                return Result<PagedResult<ProfileBasicInfoDto>>.Success(
                    new PagedResult<ProfileBasicInfoDto>
                    {
                        Data = friendDtos,
                        TotalCount = friendships.TotalCount,
                        PageNumber = friendships.PageNumber,
                        PageSize = friendships.PageSize
                    });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while getting friendships");
                return Result<PagedResult<ProfileBasicInfoDto>>.Failure("Error occurred while getting friendships");
            }
        }

    }
}
