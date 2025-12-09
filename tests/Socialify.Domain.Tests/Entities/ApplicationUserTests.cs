using FluentAssertions;
using Socialify.Domain.Entities;
using Xunit;

namespace Socialify.Domain.Tests.Entities;

public class ApplicationUserTests
{
    [Fact]
    public void NewUser_ShouldHaveDefaultProfilePicUrl()
    {
        // Act
        var user = new ApplicationUser();
        
        // Assert
        user.ProfilePicUrl.Should().Be("images/profilePics/default-profile-pic.jpg");
    }
    
    [Fact]
    public void NewUser_ShouldBeActiveByDefault()
    {
        // Act
        var user = new ApplicationUser();
        
        // Assert
        user.IsActive.Should().BeTrue();
    }
    
    [Fact]
    public void NewUser_ShouldHaveEmptyCollections()
    {
        // Act
        var user = new ApplicationUser();
        
        // Assert
        user.Posts.Should().BeEmpty();
        user.Likes.Should().BeEmpty();
        user.Comments.Should().BeEmpty();
        user.SavedPosts.Should().BeEmpty();
        user.Friendships.Should().BeEmpty();
        user.SentFriendRequests.Should().BeEmpty();
        user.ReceivedFriendRequests.Should().BeEmpty();
    }
    
    [Fact]
    public void NewUser_ShouldHaveEmptyStringDefaults()
    {
        // Act
        var user = new ApplicationUser();
        
        // Assert
        user.FirstName.Should().BeEmpty();
        user.LastName.Should().BeEmpty();
        user.FullName.Should().BeEmpty();
    }
}
