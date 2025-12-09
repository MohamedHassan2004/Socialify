using FluentAssertions;
using Socialify.Domain.Events;
using Xunit;

namespace Socialify.Domain.Tests.Events;

public class UserDeletingEventTests
{
    [Fact]
    public void Constructor_ShouldSetUserId()
    {
        // Arrange
        var userId = "user-123";
        
        // Act
        var @event = new UserDeletingEvent(userId);
        
        // Assert
        @event.UserId.Should().Be(userId);
    }
    
    [Fact]
    public void Constructor_ShouldInheritFromDomainEvent()
    {
        // Arrange
        var userId = "user-123";
        
        // Act
        var @event = new UserDeletingEvent(userId);
        
        // Assert
        @event.Should().BeAssignableTo<DomainEvent>();
        @event.EventId.Should().NotBeEmpty();
    }
}
