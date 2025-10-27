using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Socialify.Domain.Events
{
    public class OriginalPostDeletingEvent : INotification
    {
        public int PostId { get; }

        public OriginalPostDeletingEvent(int postId)
        {
            PostId = postId;
        }

    }
}
