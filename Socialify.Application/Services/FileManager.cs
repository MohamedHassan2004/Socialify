using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Socialify.Application.Services_Interfaces;
using Socialify.Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace Socialify.Application.Services
{
    public class FileManager : IFileManager
    {
        private readonly IWebHostEnvironment _env;
        private readonly ILogger<FileManager> _logger;

        public FileManager(IWebHostEnvironment env, ILogger<FileManager> logger)
        {
            _env = env;
            _logger = logger;
        }

        public async Task<Result<string>> SaveFileAsync(IFormFile file, string folderPath)
        {
            try
            {
                var uploadsFolder = Path.Combine(_env.WebRootPath, folderPath);
                Directory.CreateDirectory(uploadsFolder);

                var fileName = $"{Guid.NewGuid()}{Path.GetExtension(file.FileName)}";
                var filePath = Path.Combine(uploadsFolder, fileName);

                await using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }

                _logger.LogInformation("File uploaded: {Path}", filePath);
                return Result<string>.Success(Path.Combine(folderPath, fileName).Replace("\\", "/"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error saving file");
                return Result<string>.Failure("Error saving file");
            }
        }

        public Result DeleteFile(string relativePath)
        {
            if (string.IsNullOrWhiteSpace(relativePath))
                return Result.Failure("Path is null");

            var fullPath = GetAbsolutePath(relativePath);
            if (!File.Exists(fullPath))
            {
                _logger.LogWarning("File not found: {Path}", fullPath);
                return Result.Failure("File not found");
            }

            try
            {
                File.Delete(fullPath);
                _logger.LogInformation("File deleted: {Path}", fullPath);
                return Result.Success();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting file {Path}", fullPath);
                return Result.Failure("Error deleting file");
            }
        }

        public string GetAbsolutePath(string relativePath)
        {
            return Path.Combine(_env.WebRootPath, relativePath).Replace("\\","/");
        }
    }

}
