using FluentAssertions;
using Socialify.Domain.Entities;
using Xunit;

namespace Socialify.Domain.Tests.Entities;

public class LikeTests
{
    [Fact]
    public void NewLike_ShouldHaveDefaultValues()
    {
        // Act
        var like = new Like();
        
        // Assert
        like.Id.Should().Be(0);
        like.UserId.Should().BeEmpty();
        like.PostId.Should().Be(0);
    }
    
    [Fact]
    public void NewLike_ShouldSetLikedAtToNow()
    {
        // Arrange
        var before = DateTime.Now.AddSeconds(-1);
        
        // Act
        var like = new Like();
        
        // Assert
        var after = DateTime.Now.AddSeconds(1);
        like.LikedAt.Should().BeOnOrAfter(before).And.BeOnOrBefore(after);
    }
}
