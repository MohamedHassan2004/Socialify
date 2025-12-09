using FluentAssertions;
using Socialify.Domain.Entities;
using Xunit;

namespace Socialify.Domain.Tests.Entities;

public class FriendshipTests
{
    [Fact]
    public void NewFriendship_ShouldHaveDefaultValues()
    {
        // Act
        var friendship = new Friendship();
        
        // Assert
        friendship.Id.Should().Be(0);
        friendship.UserId.Should().BeEmpty();
        friendship.FriendId.Should().BeEmpty();
    }
    
    [Fact]
    public void NewFriendship_ShouldSetSinceToNow()
    {
        // Arrange
        var before = DateTime.Now.AddSeconds(-1);
        
        // Act
        var friendship = new Friendship();
        
        // Assert
        var after = DateTime.Now.AddSeconds(1);
        friendship.Since.Should().BeOnOrAfter(before).And.BeOnOrBefore(after);
    }
}
