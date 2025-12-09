using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Socialify.Domain.Entities
{
    public class Friendship
    {
        public int Id { get; set; }
        public string UserId { get; set; } = string.Empty;
        public string FriendId { get; set; } = string.Empty;
        public DateTime Since { get; set; } = DateTime.Now;

        public ApplicationUser User { get; set; } = null!;
        public ApplicationUser Friend { get; set; } = null!;
    }
}
