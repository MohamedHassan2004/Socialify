using FluentAssertions;
using Socialify.Domain.Entities;
using Xunit;

namespace Socialify.Domain.Tests.Entities;

public class PostTests
{
    #region Likes Count Tests
    
    [Fact]
    public void IncrementLikes_ShouldIncreaseLikesCountByOne()
    {
        // Arrange
        var post = new Post { LikesCount = 5 };
        
        // Act
        post.IncrementLikes();
        
        // Assert
        post.LikesCount.Should().Be(6);
    }
    
    [Fact]
    public void DecrementLikesCount_ShouldDecreaseLikesCountByOne()
    {
        // Arrange
        var post = new Post { LikesCount = 5 };
        
        // Act
        post.DecrementLikesCount();
        
        // Assert
        post.LikesCount.Should().Be(4);
    }
    
    [Fact]
    public void DecrementLikesCount_WhenZero_ShouldNotGoNegative()
    {
        // Arrange
        var post = new Post { LikesCount = 0 };
        
        // Act
        post.DecrementLikesCount();
        
        // Assert
        post.LikesCount.Should().Be(0);
    }
    
    #endregion
    
    #region Comments Count Tests
    
    [Fact]
    public void IncrementCommentsCount_ShouldIncreaseCommentsCountByOne()
    {
        // Arrange
        var post = new Post { CommentsCount = 3 };
        
        // Act
        post.IncrementCommentsCount();
        
        // Assert
        post.CommentsCount.Should().Be(4);
    }
    
    [Fact]
    public void DecrementCommentsCount_ShouldDecreaseCommentsCountByOne()
    {
        // Arrange
        var post = new Post { CommentsCount = 3 };
        
        // Act
        post.DecrementCommentsCount();
        
        // Assert
        post.CommentsCount.Should().Be(2);
    }
    
    [Fact]
    public void DecrementCommentsCount_WhenZero_ShouldNotGoNegative()
    {
        // Arrange
        var post = new Post { CommentsCount = 0 };
        
        // Act
        post.DecrementCommentsCount();
        
        // Assert
        post.CommentsCount.Should().Be(0);
    }
    
    #endregion
    
    #region Shares Count Tests
    
    [Fact]
    public void IncrementSharesCount_ShouldIncreaseSharesCountByOne()
    {
        // Arrange
        var post = new Post { SharesCount = 2 };
        
        // Act
        post.IncrementSharesCount();
        
        // Assert
        post.SharesCount.Should().Be(3);
    }
    
    [Fact]
    public void DecrementSharesCount_ShouldDecreaseSharesCountByOne()
    {
        // Arrange
        var post = new Post { SharesCount = 2 };
        
        // Act
        post.DecrementSharesCount();
        
        // Assert
        post.SharesCount.Should().Be(1);
    }
    
    [Fact]
    public void DecrementSharesCount_WhenZero_ShouldNotGoNegative()
    {
        // Arrange
        var post = new Post { SharesCount = 0 };
        
        // Act
        post.DecrementSharesCount();
        
        // Assert
        post.SharesCount.Should().Be(0);
    }
    
    #endregion
    
    #region Default Values Tests
    
    [Fact]
    public void NewPost_ShouldHaveDefaultValues()
    {
        // Act
        var post = new Post();
        
        // Assert
        post.LikesCount.Should().Be(0);
        post.CommentsCount.Should().Be(0);
        post.SharesCount.Should().Be(0);
        post.IsEdited.Should().BeFalse();
        post.IsShared.Should().BeFalse();
        post.UserId.Should().BeEmpty();
        post.Likes.Should().BeEmpty();
        post.Comments.Should().BeEmpty();
        post.SavedPosts.Should().BeEmpty();
        post.SharedPosts.Should().BeEmpty();
    }
    
    #endregion
}
