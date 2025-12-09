using FluentAssertions;
using Socialify.Domain.Entities;
using Xunit;

namespace Socialify.Domain.Tests.Entities;

public class CommentTests
{
    [Fact]
    public void NewComment_ShouldHaveDefaultValues()
    {
        // Act
        var comment = new Comment();
        
        // Assert
        comment.Id.Should().Be(0);
        comment.Content.Should().BeEmpty();
        comment.IsEdited.Should().BeFalse();
        comment.EditedAt.Should().BeNull();
        comment.UserId.Should().BeEmpty();
        comment.PostId.Should().Be(0);
    }
    
    [Fact]
    public void NewComment_ShouldSetCreatedAtToNow()
    {
        // Arrange
        var before = DateTime.Now.AddSeconds(-1);
        
        // Act
        var comment = new Comment();
        
        // Assert
        var after = DateTime.Now.AddSeconds(1);
        comment.CreatedAt.Should().BeOnOrAfter(before).And.BeOnOrBefore(after);
    }
}
