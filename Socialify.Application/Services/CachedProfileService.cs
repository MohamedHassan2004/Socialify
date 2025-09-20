using Microsoft.Extensions.Caching.Memory;
using Socialify.Application.Interfaces;
using Socialify.Application.DTOs.Profile;
using Socialify.Domain.Common;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Socialify.Application.Services
{
    public class CachedProfileService : IProfileService
    {
        private readonly IProfileService _profileService;
        private readonly IMemoryCache _cache;
        private readonly ILogger<CachedProfileService> _logger;
        private readonly TimeSpan _cacheExpiration = TimeSpan.FromMinutes(15);

        public CachedProfileService(
            IProfileService profileService,
            IMemoryCache cache,
            ILogger<CachedProfileService> logger)
        {
            _profileService = profileService;
            _cache = cache;
            _logger = logger;
        }

        public async Task<Result<ProfileDto>> GetUserProfileAsync(string targetUserId, string currentUserId)
        {
            var cacheKey = $"profile_{targetUserId}_{currentUserId}";
            
            if (_cache.TryGetValue(cacheKey, out Result<ProfileDto>? cachedResult))
            {
                _logger.LogDebug("Profile cache hit for user {UserId}", targetUserId);
                return cachedResult!;
            }

            var result = await _profileService.GetUserProfileAsync(targetUserId, currentUserId);
            
            if (result.IsSuccess)
            {
                _cache.Set(cacheKey, result, _cacheExpiration);
                _logger.LogDebug("Profile cached for user {UserId}", targetUserId);
            }

            return result;
        }

        // Implement other methods by delegating to the underlying service
        public async Task<Result> UpdateProfileInfoAsync(string currentUserId, UpdateProfileInfoDto updateProfileInfoDto)
        {
            var result = await _profileService.UpdateProfileInfoAsync(currentUserId, updateProfileInfoDto);
            
            if (result.IsSuccess)
            {
                // Invalidate cache for this user
                InvalidateUserCache(currentUserId);
            }
            
            return result;
        }

        public async Task<Result<UpdateProfileInfoDto>> GetProfileInfoAsync(string currentUserId)
        {
            return await _profileService.GetProfileInfoAsync(currentUserId);
        }

        public async Task<Result<ProfileBasicInfoDto>> GetProfileBasicInfoAsync(string currentUserId)
        {
            return await _profileService.GetProfileBasicInfoAsync(currentUserId);
        }

        public async Task<Result> RemoveProfilePictureAsync(string currentUserId)
        {
            var result = await _profileService.RemoveProfilePictureAsync(currentUserId);
            
            if (result.IsSuccess)
            {
                InvalidateUserCache(currentUserId);
            }
            
            return result;
        }

        public async Task<Result> UpdateProfilePictureAsync(string currentUserId, PatchProfilePicDto patchProfilePicDto)
        {
            var result = await _profileService.UpdateProfilePictureAsync(currentUserId, patchProfilePicDto);
            
            if (result.IsSuccess)
            {
                InvalidateUserCache(currentUserId);
            }
            
            return result;
        }

        private void InvalidateUserCache(string userId)
        {
            // Remove all cache entries for this user
            var keysToRemove = new List<string>();
            var cacheField = typeof(MemoryCache).GetField("_coherentState", 
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            
            if (cacheField?.GetValue(_cache) is object coherentState)
            {
                var entriesCollection = coherentState.GetType()
                    .GetProperty("EntriesCollection", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                    ?.GetValue(coherentState);
                
                if (entriesCollection is IDictionary<string, object> entries)
                {
                    foreach (var key in entries.Keys)
                    {
                        if (key.Contains($"profile_{userId}"))
                        {
                            keysToRemove.Add(key);
                        }
                    }
                }
            }

            foreach (var key in keysToRemove)
            {
                _cache.Remove(key);
            }

            _logger.LogDebug("Cache invalidated for user {UserId}", userId);
        }
    }
}
