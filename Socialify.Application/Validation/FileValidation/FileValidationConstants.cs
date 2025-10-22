using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Socialify.Application.Validation.FileValidation
{
    public class FileValidationConstants
    {
        // Images
        public static readonly string[] ImageExtensions = { ".jpg", ".jpeg", ".png", ".gif", ".webp" };
        public static readonly string[] ImageMimeTypes =
        {
            "image/jpeg", "image/png", "image/gif", "image/webp"
        };

        // Videos
        public static readonly string[] VideoExtensions = { ".mp4", ".mov", ".avi", ".mkv", ".webm" };
        public static readonly string[] VideoMimeTypes =
        {
            "video/mp4", "video/quicktime", "video/x-msvideo", "video/x-matroska", "video/webm"
        };

        // Audios
        public static readonly string[] AudioExtensions = { ".mp3", ".wav", ".ogg", ".aac", ".flac" };
        public static readonly string[] AudioMimeTypes =
        {
            "audio/mpeg", "audio/wav", "audio/ogg", "audio/aac", "audio/flac"
        };

        // Mixed (images + videos + audios) for general post media
        public static readonly string[] MediaExtensions =
            ImageExtensions.Concat(VideoExtensions).Concat(AudioExtensions).ToArray();

        public static readonly string[] MediaMimeTypes =
            ImageMimeTypes.Concat(VideoMimeTypes).Concat(AudioMimeTypes).ToArray();
    }
}
