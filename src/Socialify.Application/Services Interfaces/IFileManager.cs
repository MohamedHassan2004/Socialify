using Microsoft.AspNetCore.Http;
using Socialify.Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Socialify.Application.Services_Interfaces
{
    public interface IFileManager
    {
        Task<Result<string>> SaveFileAsync(IFormFile file, string folderPath);
        Result DeleteFile(string relativePath);
        string GetAbsolutePath(string relativePath);
    }

}
