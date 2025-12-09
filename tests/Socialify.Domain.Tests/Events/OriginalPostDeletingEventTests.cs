using FluentAssertions;
using Socialify.Domain.Events;
using MediatR;
using Xunit;

namespace Socialify.Domain.Tests.Events;

public class OriginalPostDeletingEventTests
{
    [Fact]
    public void Constructor_ShouldSetPostId()
    {
        // Arrange
        var postId = 42;
        
        // Act
        var @event = new OriginalPostDeletingEvent(postId);
        
        // Assert
        @event.PostId.Should().Be(postId);
    }
    
    [Fact]
    public void Event_ShouldImplementINotification()
    {
        // Arrange
        var postId = 42;
        
        // Act
        var @event = new OriginalPostDeletingEvent(postId);
        
        // Assert
        @event.Should().BeAssignableTo<INotification>();
    }
}
