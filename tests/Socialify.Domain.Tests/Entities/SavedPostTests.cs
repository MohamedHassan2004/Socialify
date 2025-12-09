using FluentAssertions;
using Socialify.Domain.Entities;
using Xunit;

namespace Socialify.Domain.Tests.Entities;

public class SavedPostTests
{
    [Fact]
    public void NewSavedPost_ShouldHaveDefaultValues()
    {
        // Act
        var savedPost = new SavedPost();
        
        // Assert
        savedPost.Id.Should().Be(0);
        savedPost.UserId.Should().BeEmpty();
        savedPost.PostId.Should().Be(0);
    }
    
    [Fact]
    public void NewSavedPost_ShouldSetSavedAtToNow()
    {
        // Arrange
        var before = DateTime.Now.AddSeconds(-1);
        
        // Act
        var savedPost = new SavedPost();
        
        // Assert
        var after = DateTime.Now.AddSeconds(1);
        savedPost.SavedAt.Should().BeOnOrAfter(before).And.BeOnOrBefore(after);
    }
}
