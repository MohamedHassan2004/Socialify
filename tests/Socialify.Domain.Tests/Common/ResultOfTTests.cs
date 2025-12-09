using FluentAssertions;
using Socialify.Domain.Common;
using Xunit;

namespace Socialify.Domain.Tests.Common;

public class ResultOfTTests
{
    [Fact]
    public void Success_ShouldSetIsSuccessToTrue()
    {
        // Arrange
        var data = "test data";
        
        // Act
        var result = Result<string>.Success(data);
        
        // Assert
        result.IsSuccess.Should().BeTrue();
    }
    
    [Fact]
    public void Success_ShouldContainData()
    {
        // Arrange
        var data = "test data";
        
        // Act
        var result = Result<string>.Success(data);
        
        // Assert
        result.Data.Should().Be(data);
    }
    
    [Fact]
    public void Success_ShouldHaveEmptyErrorMessage()
    {
        // Arrange
        var data = "test data";
        
        // Act
        var result = Result<string>.Success(data);
        
        // Assert
        result.ErrorMessage.Should().BeEmpty();
        result.Errors.Should().BeEmpty();
    }
    
    [Fact]
    public void Failure_WithMessage_ShouldSetIsSuccessToFalse()
    {
        // Act
        var result = Result<string>.Failure("An error occurred");
        
        // Assert
        result.IsSuccess.Should().BeFalse();
    }
    
    [Fact]
    public void Failure_WithMessage_ShouldContainErrorMessage()
    {
        // Arrange
        var errorMessage = "An error occurred";
        
        // Act
        var result = Result<string>.Failure(errorMessage);
        
        // Assert
        result.ErrorMessage.Should().Be(errorMessage);
    }
    
    [Fact]
    public void Failure_WithMessage_ShouldHaveNullData()
    {
        // Act
        var result = Result<string>.Failure("An error occurred");
        
        // Assert
        result.Data.Should().BeNull();
    }
    
    [Fact]
    public void Failure_WithList_ShouldContainErrors()
    {
        // Arrange
        var errors = new List<string> { "Error 1", "Error 2", "Error 3" };
        
        // Act
        var result = Result<string>.Failure(errors);
        
        // Assert
        result.Errors.Should().BeEquivalentTo(errors);
        result.IsSuccess.Should().BeFalse();
    }
    
    [Fact]
    public void Success_WithComplexType_ShouldContainData()
    {
        // Arrange
        var data = new { Name = "Test", Value = 42 };
        
        // Act
        var result = Result<object>.Success(data);
        
        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Data.Should().Be(data);
    }
}
