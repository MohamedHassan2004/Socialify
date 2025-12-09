using FluentAssertions;
using Socialify.Domain.Entities;
using Xunit;

namespace Socialify.Domain.Tests.Entities;

public class SharedPostTests
{
    [Fact]
    public void NewSharedPost_ShouldHaveDefaultValues()
    {
        // Act
        var sharedPost = new SharedPost();
        
        // Assert
        sharedPost.Id.Should().Be(0);
        sharedPost.OriginalPostId.Should().Be(0);
        sharedPost.SharedPostId.Should().Be(0);
        sharedPost.SharedByUserId.Should().BeEmpty();
    }
    
    [Fact]
    public void NewSharedPost_ShouldSetSharedAtToNow()
    {
        // Arrange
        var before = DateTime.Now.AddSeconds(-1);
        
        // Act
        var sharedPost = new SharedPost();
        
        // Assert
        var after = DateTime.Now.AddSeconds(1);
        sharedPost.SharedAt.Should().BeOnOrAfter(before).And.BeOnOrBefore(after);
    }
}
