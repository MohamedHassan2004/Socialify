using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Socialify.Application.DTOs.Notification
{
    public class NotificationDto
    {
        public int Id { get; set; }
        public string UserId { get; set; } 
        public string UserName { get; set; }
        public string UserProfilePicUrl { get; set; }
        public string Message { get; set; } = string.Empty;
        public string Link { get; set; } = string.Empty;
        public string TimeAgo { get; set; } = string.Empty;
        public bool IsRead { get; set; } = false;
    }
}
