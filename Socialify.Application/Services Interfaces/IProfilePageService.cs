using Socialify.Application.DTOs.Profile;
using Socialify.Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Socialify.Application.Services_Interfaces
{
    public interface IProfilePageService
    {
        Task<Result<ProfilePageDto>> GetProfilePageAsync(string targetUserId, string currentUserId);
    }
}
