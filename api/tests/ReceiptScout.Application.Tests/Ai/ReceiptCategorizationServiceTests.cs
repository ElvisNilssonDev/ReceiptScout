using NSubstitute;
using ReceiptScout.Application.Ai;
using ReceiptScout.Application.Ai.Dtos;
using ReceiptScout.Application.Common.Interfaces;
using ReceiptScout.Domain.Entities;
using Shouldly;
using Xunit;

namespace ReceiptScout.Application.Tests.Ai;

public class ReceiptCategorizationServiceTests
{
    [Fact]
    public async Task SuggestForReceiptAsync_BuildsGroundedRequest_AndReturnsAiSuggestion()
    {
        // Arrange — a fuel receipt owned by "owner-1" and one candidate category.
        var receipt = new Receipt("Circle K", DateTime.UtcNow, 600m, 120m, "owner-1", "Tankning");
        var fuel = new Category("Drivmedel", "5611", 0.25m);

        var receipts = Substitute.For<IReceiptRepository>();
        receipts.GetByIdAsync(receipt.Id).Returns(receipt);

        var categories = Substitute.For<ICategoryRepository>();
        categories.GetAllAsync().Returns(new List<Category> { fuel });

        var currentUser = Substitute.For<ICurrentUserService>();
        currentUser.UserId.Returns("owner-1");

        var ai = Substitute.For<IAiCategorizationService>();
        var expected = new CategorySuggestion(fuel.Id, "5611", 0.92, "Looks like fuel.");
        ai.SuggestCategoryAsync(Arg.Any<ReceiptCategorizationRequest>(), Arg.Any<CancellationToken>())
          .Returns(expected);

        var sut = new ReceiptCategorizationService(receipts, categories, ai, currentUser);

        // Act
        var result = await sut.SuggestForReceiptAsync(receipt.Id);

        // Assert — the AI's answer flows straight back...
        result.ShouldBe(expected);

        // ...and we handed the AI a request built from the receipt and grounded with
        // the available categories. This asserts OUR wiring, not the mock.
        await ai.Received(1).SuggestCategoryAsync(
            Arg.Is<ReceiptCategorizationRequest>(r =>
                r.Merchant == "Circle K" &&
                r.Candidates.Count == 1 &&
                r.Candidates[0].BasAccount == "5611"),
            Arg.Any<CancellationToken>());
    }
}