using Socialify.Application.DTOs.Home;
using Socialify.Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Socialify.Application.Services_Interfaces
{
    public interface IHomePageService
    {
        Task<Result<HomePageDto>> GetHomePageAsync(string currentUserId);
    }
}
