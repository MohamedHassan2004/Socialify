using Socialify.Domain.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Socialify.Domain.Entities
{
    public class Notification
    {
        public int Id { get; set; }
        public string ReceiverUserId { get; set; } = string.Empty;
        public string UserId { get; set; } = string.Empty;
        [ForeignKey("UserId")]
        public ApplicationUser User { get; set; } = null!;
        [ForeignKey("ReceiverUserId")]
        public ApplicationUser ReceiverUser { get; set; } = null!;
        public NotificationType NotificationType { get; set; }
        public int? PostId { get; set; }
        public Post? Post { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public bool IsRead { get; set; } = false;
    }
}
