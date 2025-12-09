using FluentAssertions;
using Socialify.Domain.Common;
using Xunit;

namespace Socialify.Domain.Tests.Common;

public class ResultTests
{
    [Fact]
    public void Success_ShouldSetIsSuccessToTrue()
    {
        // Act
        var result = Result.Success();
        
        // Assert
        result.IsSuccess.Should().BeTrue();
    }
    
    [Fact]
    public void Success_ShouldHaveEmptyErrorMessage()
    {
        // Act
        var result = Result.Success();
        
        // Assert
        result.ErrorMessage.Should().BeEmpty();
        result.Errors.Should().BeEmpty();
    }
    
    [Fact]
    public void Failure_WithMessage_ShouldSetIsSuccessToFalse()
    {
        // Act
        var result = Result.Failure("An error occurred");
        
        // Assert
        result.IsSuccess.Should().BeFalse();
    }
    
    [Fact]
    public void Failure_WithMessage_ShouldContainErrorMessage()
    {
        // Arrange
        var errorMessage = "An error occurred";
        
        // Act
        var result = Result.Failure(errorMessage);
        
        // Assert
        result.ErrorMessage.Should().Be(errorMessage);
    }
    
    [Fact]
    public void Failure_WithList_ShouldContainErrors()
    {
        // Arrange
        var errors = new List<string> { "Error 1", "Error 2", "Error 3" };
        
        // Act
        var result = Result.Failure(errors);
        
        // Assert
        result.Errors.Should().BeEquivalentTo(errors);
        result.IsSuccess.Should().BeFalse();
    }
    
    [Fact]
    public void Failure_WithList_ShouldHaveEmptyErrorMessage()
    {
        // Arrange
        var errors = new List<string> { "Error 1", "Error 2" };
        
        // Act
        var result = Result.Failure(errors);
        
        // Assert
        result.ErrorMessage.Should().BeEmpty();
    }
}
