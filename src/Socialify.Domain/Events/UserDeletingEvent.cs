using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Socialify.Domain.Events
{
    public class UserDeletingEvent : DomainEvent
    {
        public string UserId { get; }
        public UserDeletingEvent(string userId)
        {
            UserId = userId;
        }
    }
}
