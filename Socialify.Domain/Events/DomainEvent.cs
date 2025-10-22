using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Socialify.Domain.Events
{
    public abstract class DomainEvent : INotification
    {
        public Guid EventId { get; }
        public DateTime OccurredOn { get; }
        protected DomainEvent()
        {
            OccurredOn = DateTime.UtcNow;
            EventId = Guid.NewGuid();
        }
    }
}
