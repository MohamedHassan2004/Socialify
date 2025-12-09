using FluentAssertions;
using Socialify.Domain.Entities;
using Xunit;

namespace Socialify.Domain.Tests.Entities;

public class FriendRequestTests
{
    [Fact]
    public void NewFriendRequest_ShouldHaveDefaultValues()
    {
        // Act
        var request = new FriendRequest();
        
        // Assert
        request.Id.Should().Be(0);
        request.SenderId.Should().BeEmpty();
        request.ReceiverId.Should().BeEmpty();
    }
    
    [Fact]
    public void NewFriendRequest_ShouldSetSentAtToNow()
    {
        // Arrange
        var before = DateTime.Now.AddSeconds(-1);
        
        // Act
        var request = new FriendRequest();
        
        // Assert
        var after = DateTime.Now.AddSeconds(1);
        request.SentAt.Should().BeOnOrAfter(before).And.BeOnOrBefore(after);
    }
}
