using FluentAssertions;
using Socialify.Domain.Events;
using Xunit;

namespace Socialify.Domain.Tests.Events;

/// <summary>
/// Tests for DomainEvent base class using UserDeletingEvent as concrete implementation
/// </summary>
public class DomainEventTests
{
    [Fact]
    public void NewEvent_ShouldGenerateEventId()
    {
        // Act
        var domainEvent = new UserDeletingEvent("user-123");
        
        // Assert
        domainEvent.EventId.Should().NotBeEmpty();
    }
    
    [Fact]
    public void NewEvent_ShouldGenerateUniqueEventIds()
    {
        // Act
        var event1 = new UserDeletingEvent("user-123");
        var event2 = new UserDeletingEvent("user-456");
        
        // Assert
        event1.EventId.Should().NotBe(event2.EventId);
    }
    
    [Fact]
    public void NewEvent_ShouldSetOccurredOnToCurrentTime()
    {
        // Arrange
        var before = DateTime.Now.AddSeconds(-1);
        
        // Act
        var domainEvent = new UserDeletingEvent("user-123");
        
        // Assert
        var after = DateTime.Now.AddSeconds(1);
        domainEvent.OccurredOn.Should().BeOnOrAfter(before).And.BeOnOrBefore(after);
    }
}
