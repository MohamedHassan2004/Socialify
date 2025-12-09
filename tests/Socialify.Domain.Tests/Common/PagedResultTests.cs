using FluentAssertions;
using Socialify.Application.DTOs.Common;
using Xunit;

namespace Socialify.Domain.Tests.Common;

public class PagedResultTests
{
    #region TotalPages Tests
    
    [Fact]
    public void TotalPages_ShouldCalculateCorrectly_WhenEvenlyDivisible()
    {
        // Arrange
        var pagedResult = new PagedResult<string>
        {
            TotalCount = 100,
            PageSize = 10
        };
        
        // Assert
        pagedResult.TotalPages.Should().Be(10);
    }
    
    [Fact]
    public void TotalPages_ShouldRoundUp_WhenNotEvenlyDivisible()
    {
        // Arrange
        var pagedResult = new PagedResult<string>
        {
            TotalCount = 25,
            PageSize = 10
        };
        
        // Assert
        pagedResult.TotalPages.Should().Be(3);
    }
    
    [Fact]
    public void TotalPages_ShouldBeZero_WhenTotalCountIsZero()
    {
        // Arrange
        var pagedResult = new PagedResult<string>
        {
            TotalCount = 0,
            PageSize = 10
        };
        
        // Assert
        pagedResult.TotalPages.Should().Be(0);
    }
    
    [Fact]
    public void TotalPages_ShouldBeOne_WhenTotalCountLessThanPageSize()
    {
        // Arrange
        var pagedResult = new PagedResult<string>
        {
            TotalCount = 5,
            PageSize = 10
        };
        
        // Assert
        pagedResult.TotalPages.Should().Be(1);
    }
    
    #endregion
    
    #region HasPreviousPage Tests
    
    [Fact]
    public void HasPreviousPage_ShouldBeFalse_OnFirstPage()
    {
        // Arrange
        var pagedResult = new PagedResult<string>
        {
            PageNumber = 1,
            PageSize = 10,
            TotalCount = 50
        };
        
        // Assert
        pagedResult.HasPreviousPage.Should().BeFalse();
    }
    
    [Fact]
    public void HasPreviousPage_ShouldBeTrue_OnSecondPage()
    {
        // Arrange
        var pagedResult = new PagedResult<string>
        {
            PageNumber = 2,
            PageSize = 10,
            TotalCount = 50
        };
        
        // Assert
        pagedResult.HasPreviousPage.Should().BeTrue();
    }
    
    [Fact]
    public void HasPreviousPage_ShouldBeFalse_WhenPageNumberIsZero()
    {
        // Arrange
        var pagedResult = new PagedResult<string>
        {
            PageNumber = 0,
            PageSize = 10,
            TotalCount = 50
        };
        
        // Assert
        pagedResult.HasPreviousPage.Should().BeFalse();
    }
    
    #endregion
    
    #region HasNextPage Tests
    
    [Fact]
    public void HasNextPage_ShouldBeTrue_WhenMorePagesExist()
    {
        // Arrange
        var pagedResult = new PagedResult<string>
        {
            PageNumber = 1,
            PageSize = 10,
            TotalCount = 50
        };
        
        // Assert
        pagedResult.HasNextPage.Should().BeTrue();
    }
    
    [Fact]
    public void HasNextPage_ShouldBeFalse_OnLastPage()
    {
        // Arrange
        var pagedResult = new PagedResult<string>
        {
            PageNumber = 5,
            PageSize = 10,
            TotalCount = 50
        };
        
        // Assert
        pagedResult.HasNextPage.Should().BeFalse();
    }
    
    [Fact]
    public void HasNextPage_ShouldBeFalse_BeyondLastPage()
    {
        // Arrange
        var pagedResult = new PagedResult<string>
        {
            PageNumber = 10,
            PageSize = 10,
            TotalCount = 50
        };
        
        // Assert
        pagedResult.HasNextPage.Should().BeFalse();
    }
    
    #endregion
    
    #region Default Values Tests
    
    [Fact]
    public void NewPagedResult_ShouldHaveEmptyData()
    {
        // Act
        var pagedResult = new PagedResult<string>();
        
        // Assert
        pagedResult.Data.Should().BeEmpty();
    }
    
    #endregion
}
