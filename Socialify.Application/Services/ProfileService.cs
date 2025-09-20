using AutoMapper;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Socialify.Application.DTOs.Profile;
using Socialify.Application.Interfaces;
using Socialify.Application.ReposInterfaces;
using Socialify.Domain.Common;
using Socialify.Domain.Entities;
using Socialify.Domain.Enums;
using System;
using System.IO;
using System.Threading.Tasks;

namespace Socialify.Application.Services;

public class ProfileService : IProfileService
{
    private readonly IProfileRepository _profileRepository;
    private readonly IMapper _mapper;
    private readonly IWebHostEnvironment _env;
    private readonly ILogger<ProfileService> _logger;
    private readonly string _defaultProfilePic;
    private readonly string _profilePicsPath = Path.Combine("images", "profilePics");

    public ProfileService(
        IProfileRepository profileRepository,
        IMapper mapper,
        IWebHostEnvironment env,
        ILogger<ProfileService> logger,
        IConfiguration config)
    {
        _profileRepository = profileRepository;
        _mapper = mapper;
        _env = env;
        _logger = logger;
        _defaultProfilePic = config["ProfileSettings:DefaultProfilePic"]
                            ?? "images/profilePics/default-profile-pic.jpg";
    }

    public async Task<Result<ProfileDto>> GetUserProfileAsync(string targetUserId, string currentUserId)
    {
        try
        {
            // Use optimized method that loads user with posts in single query
            var user = await _profileRepository.GetByIdWithPostsAsync(targetUserId);
            if (user == null)
            {
                _logger.LogWarning("User not found: {UserId}", targetUserId);
                return Result<ProfileDto>.Failure("User not found");
            }

            _logger.LogInformation("Fetched profile for user {UserId} successfully", targetUserId);

            var isCurrentUser = targetUserId == currentUserId;
            var profileDto = MapProfile(user, isCurrentUser);

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
            // Use tracking query for updates
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
            // For profile info, we don't need related data, so use simple query
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
            // For basic info, we don't need related data
            var user = await _profileRepository.GetByIdAsync(currentUserId);
            if (user == null)
            {
                return Result<ProfileBasicInfoDto>.Failure("User not found");
            }

            var profileBasicInfoDto = _mapper.Map<ProfileBasicInfoDto>(user);
            _logger.LogInformation("Fetched basic info for user {UserId} successfully", currentUserId);
            return Result<ProfileBasicInfoDto>.Success(profileBasicInfoDto);
        }
        catch(Exception ex)
        {
            _logger.LogError(ex, "Error fetching basic info for user {UserId}", currentUserId);
            return Result<ProfileBasicInfoDto>.Failure("An error occurred while fetching the basic info.");
        }
    }

    // New method to get multiple profiles efficiently
    public async Task<Result<IEnumerable<ProfileDto>>> GetMultipleProfilesAsync(
        IEnumerable<string> userIds, 
        string currentUserId)
    {
        try
        {
            var users = await _profileRepository.GetUsersWithPostsAsync(userIds);
            var profiles = users.Select(user => MapProfile(user, user.Id == currentUserId));
            
            _logger.LogInformation("Fetched {Count} profiles successfully", profiles.Count());
            return Result<IEnumerable<ProfileDto>>.Success(profiles);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching multiple profiles");
            return Result<IEnumerable<ProfileDto>>.Failure("An error occurred while fetching profiles.");
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

            DeleteProfilePictureFile(user);

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

            DeleteProfilePictureFile(user);

            var newImagePath = await UploadProfilePictureAsync(img);
            user.ProfilePicUrl = newImagePath;

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

    private void DeleteProfilePictureFile(ApplicationUser user)
    {
        if (user.ProfilePicUrl == _defaultProfilePic)
        {
            return;
        }

        var fullPath = Path.Combine(_env.WebRootPath, user.ProfilePicUrl);
        if (!File.Exists(fullPath))
        {
            _logger.LogWarning("Profile picture file not found for user {UserId} at path: {ProfilePicUrl}", user.Id, user.ProfilePicUrl);
            return;
        }
        
        try
        {
            File.Delete(fullPath);
            _logger.LogInformation("Successfully deleted old profile picture for user {UserId} at {Path}", user.Id, fullPath);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting profile picture file for user {UserId} at {Path}", user.Id, fullPath);
        }
    }

    private async Task<string> UploadProfilePictureAsync(IFormFile newPicture)
    {
        var uploadsFolder = Path.Combine(_env.WebRootPath, _profilePicsPath);
        Directory.CreateDirectory(uploadsFolder);

        var fileName = $"{Guid.NewGuid()}{Path.GetExtension(newPicture.FileName)}";
        var filePath = Path.Combine(uploadsFolder, fileName);

        await using (var stream = new FileStream(filePath, FileMode.Create))
        {
            await newPicture.CopyToAsync(stream);
        }

        _logger.LogInformation("Successfully uploaded new profile picture to {Path}", filePath);

        return Path.Combine(_profilePicsPath, fileName).Replace("\\", "/");
    }

    private ProfileDto MapProfile(ApplicationUser user, bool isCurrentUser)
    {
        var profileDto = _mapper.Map<ProfileDto>(user);
        profileDto.IsCurrentUser = isCurrentUser;
        if(isCurrentUser)
        {
            profileDto.Status = RelationshipStatus.Self;
        }
        else
        {
            profileDto.Status = RelationshipStatus.None;
        }

        _logger.LogInformation("Mapped ProfileDto to profile entity for user {UserId}", user.Id);
        return profileDto;
    }
}
