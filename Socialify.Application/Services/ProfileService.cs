using AutoMapper;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Socialify.Application.DTOs.Common;
using Socialify.Application.DTOs.Post;
using Socialify.Application.DTOs.Profile;
using Socialify.Application.Interfaces;
using Socialify.Application.ReposInterfaces;
using Socialify.Application.Services_Interfaces;
using Socialify.Domain.Common;
using Socialify.Domain.Entities;
using Socialify.Domain.Enums;
using System;
using System.Diagnostics.Eventing.Reader;
using System.IO;
using System.Threading.Tasks;

namespace Socialify.Application.Services;

public class ProfileService : IProfileService
{
    private readonly IProfileRepository _profileRepository;
    private readonly IMapper _mapper;
    private readonly ILogger<ProfileService> _logger;
    private readonly IFileManager _fileManager;
    private readonly string _defaultProfilePic;
    private readonly string _profilePicsPath;

    public ProfileService(
        IProfileRepository profileRepository,
        IMapper mapper,
        ILogger<ProfileService> logger,
        IConfiguration config,
        IFileManager fileManager)
    {
        _profileRepository = profileRepository;
        _mapper = mapper;
        _logger = logger;
        _fileManager = fileManager;
        _profilePicsPath = config["FileSettings:ProfilePicsPath"]
                            ?? "images/profilePics";

        _defaultProfilePic = config["ProfileSettings:DefaultProfilePic"]
                            ?? Path.Combine(_profilePicsPath, "default-profile-pic.jpg");
    }

    public async Task<Result<ProfileDto>> GetUserProfileAsync(string targetUserId, string currentUserId)
    {
        try
        {
            var user = await _profileRepository.GetUserProfileByIdAsync(targetUserId);
            if (user == null)
            {
                _logger.LogWarning("User not found: {UserId}", targetUserId);
                return Result<ProfileDto>.Failure("User not found");
            }


            var isCurrentUser = targetUserId == currentUserId;
            var profileDto = MapProfile(user, currentUserId);

            if(isCurrentUser)
                _logger.LogInformation("Fetched profile for user {UserId} successfully", targetUserId);
            else
                _logger.LogInformation("Fetched profile for user {UserId} by user {CurrentUserId} successfully", targetUserId, currentUserId);
            return Result<ProfileDto>.Success(profileDto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching profile for user {UserId}", targetUserId);
            return Result<ProfileDto>.Failure("An error occurred while fetching the profile.");
        }
    }

    public async Task<Result> UpdateProfileInfoAsync(string currentUserId, UpdateProfileInfoDto updateProfileInfoDto)
    {
        try 
        {
            var user = await _profileRepository.GetByIdAsync(currentUserId);
            if (user == null)
            {
                return Result.Failure("User not found");
            }

            _mapper.Map(updateProfileInfoDto, user);
            await _profileRepository.SaveChangesAsync();
            _logger.LogInformation("User {UserId} updated profile info successfully", user.Id);
            return Result.Success();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating profile info for user {UserId}", currentUserId);
            return Result.Failure("An error occurred while updating the profile info.");
        }
    }

    public async Task<Result<UpdateProfileInfoDto>> GetProfileInfoAsync(string currentUserId)
    {
        try
        {
            var user = await _profileRepository.GetByIdAsync(currentUserId);
            if (user == null)
            {
                return Result<UpdateProfileInfoDto>.Failure("User not found");
            }

            var updateProfileInfoDto = _mapper.Map<UpdateProfileInfoDto>(user);
            _logger.LogInformation("Fetched profile info for user {UserId} successfully", currentUserId);
            return Result<UpdateProfileInfoDto>.Success(updateProfileInfoDto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching profile info for user {UserId}", currentUserId);
            return Result<UpdateProfileInfoDto>.Failure("An error occurred while fetching the profile info.");
        }
    }

    public async Task<Result<ProfileBasicInfoDto>> GetProfileBasicInfoAsync(string currentUserId)
    {
        try
        {
            var user = await _profileRepository.GetByIdAsync(currentUserId);
            if (user == null)
            {
                return Result<ProfileBasicInfoDto>.Failure("User not found");
            }

            var profileBasicInfoDto = _mapper.Map<ProfileBasicInfoDto>(user);
            return Result<ProfileBasicInfoDto>.Success(profileBasicInfoDto);
        }
        catch(Exception ex)
        {
            _logger.LogError(ex, "Error fetching basic info for user {UserId}", currentUserId);
            return Result<ProfileBasicInfoDto>.Failure("An error occurred while fetching the basic info.");
        }
    }

    public async Task<Result<PagedResult<ProfileBasicInfoDto>>> SearchProfilesAsync(string query, int page, int pageSize)
    {
        try
        {
            var profiles = await _profileRepository.SearchUsersAsync(query,page,pageSize);
            var profileDtos = _mapper.Map<List<ProfileBasicInfoDto>>(profiles.Data);

            var pagedResult = new PagedResult<ProfileBasicInfoDto>
            {
                Data = profileDtos,
                PageNumber = page,
                PageSize = pageSize,
                TotalCount = profiles.TotalCount
            };

            _logger.LogInformation("Profiles search completed for query: {Query}", query);
            return Result<PagedResult<ProfileBasicInfoDto>>.Success(pagedResult);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while searching for profiles with query: {Query}", query);
            return Result<PagedResult<ProfileBasicInfoDto>>.Failure("An error occurred while searching for profiles.");
        }
    }

    public async Task<Result> RemoveProfilePictureAsync(string currentUserId)
    {
        try
        {
            var user = await _profileRepository.GetByIdAsync(currentUserId);
            if (user == null)
            {
                return Result.Failure("User not found");
            }

            if (user.ProfilePicUrl == _defaultProfilePic)
            {
                _logger.LogWarning("User {UserId} attempted to remove the default profile picture.", user.Id);
                return Result.Failure("The default profile picture cannot be removed.");
            }

            var deletingResult = _fileManager.DeleteFile(user.ProfilePicUrl);
            if (!deletingResult.IsSuccess)
            {
                _logger.LogWarning("Failed to delete old profile picture for user {UserId}: {ErrorMessage}", user.Id, deletingResult.ErrorMessage);
                return Result.Failure(deletingResult.ErrorMessage);
            }

            user.ProfilePicUrl = _defaultProfilePic;
            await _profileRepository.SaveChangesAsync();

            _logger.LogInformation("User {UserId} removed profile picture successfully", user.Id);
            return Result.Success();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error removing profile picture for user {UserId}", currentUserId);
            return Result.Failure("An error occurred while removing the profile picture.");
        }
    }

    public async Task<Result> UpdateProfilePictureAsync(string currentUserId, PatchProfilePicDto patchProfilePicDto)
    {
        try
        {
            var img = patchProfilePicDto.ProfilePicture;
            if (img == null || img.Length == 0)
            {
                return Result.Failure("No image file provided.");
            }

            var user = await _profileRepository.GetByIdAsync(currentUserId);
            if (user == null)
            {
                return Result.Failure("User not found");
            }

            if (user.ProfilePicUrl != _defaultProfilePic)
            {
                var deletingResult = _fileManager.DeleteFile(user.ProfilePicUrl);
                if (!deletingResult.IsSuccess)
                {
                    _logger.LogWarning("Failed to delete old profile picture for user {UserId}: {ErrorMessage}", user.Id, deletingResult.ErrorMessage);
                    return Result.Failure(deletingResult.ErrorMessage);
                }
            }

            var uploadImgResult = await _fileManager.SaveFileAsync(img,_profilePicsPath);
            if (!uploadImgResult.IsSuccess || string.IsNullOrEmpty(uploadImgResult.Data))
            {
                _logger.LogWarning("Failed to upload new profile picture for user {UserId}: {ErrorMessage}", user.Id, uploadImgResult.ErrorMessage);
                return Result.Failure(uploadImgResult.ErrorMessage);
            }
            user.ProfilePicUrl = uploadImgResult.Data;
            await _profileRepository.SaveChangesAsync();

            _logger.LogInformation("User {UserId} updated profile picture successfully", currentUserId);
            return Result.Success();
        }
        catch(Exception ex)
        {
            _logger.LogError(ex, "Error updating profile picture for user {UserId}", currentUserId);
            return Result.Failure("An error occurred while updating the profile picture.");
        }   
    }

    private ProfileDto MapProfile(ApplicationUser user, string currentUserId)
    {
        var profileDto = _mapper.Map<ProfileDto>(user);
        profileDto.IsCurrentUser = currentUserId == user.Id;
        profileDto.Status = GetRelationshipStatus(user.Id, currentUserId);
        return profileDto;
    }

    private RelationshipStatus GetRelationshipStatus(string userId, string currentUserId) {
        if (currentUserId == userId)
        {
            return RelationshipStatus.Self;
        }
        //else if()
        //{ 
        //}
        return RelationshipStatus.None;
    }

}
