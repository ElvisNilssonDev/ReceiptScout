using NSubstitute;
using ReceiptScout.Application.Categories;
using ReceiptScout.Application.Categories.Dtos;
using ReceiptScout.Application.Common.Interfaces;
using Shouldly;
using Xunit;

namespace ReceiptScout.Application.Tests.Categories;

public class CategoryServiceTests
{
    [Fact]
    public async Task CreateAsync_GeneratesSlugFromName()
    {
        // Arrange
        var repo = Substitute.For<ICategoryRepository>();
        repo.ExistsByNameAsync(Arg.Any<string>()).Returns(false);
        var currentUser = Substitute.For<ICurrentUserService>();
        currentUser.IsAdmin.Returns(true);
        var sut = new CategoryService(repo, currentUser);

        // Act
        var result = await sut.CreateAsync(
            new CreateCategoryDto("Office & Supplies", "6110", 0.25m));

        // Assert
        result.Slug.ShouldBe("office-supplies");
    }
}
