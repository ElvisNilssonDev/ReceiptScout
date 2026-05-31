using NSubstitute;
using ReceiptScout.Application.Common.Exceptions;
using ReceiptScout.Application.Common.Interfaces;
using ReceiptScout.Application.Receipts;
using ReceiptScout.Application.Receipts.Dtos;
using ReceiptScout.Domain.Entities;
using ReceiptScout.Domain.Exceptions;
using Shouldly;
using Xunit;

namespace ReceiptScout.Application.Tests.Receipts;

public class ReceiptServiceTests
{
    // Helpers — keep the test bodies focused on what they're actually testing
    private static CreateReceiptDto SampleCreateDto() => new(
        Merchant: "ICA",
        Date: new DateTime(2026, 5, 1, 12, 0, 0, DateTimeKind.Utc),
        TotalAmount: 250m,
        VatAmount: 50m,
        Description: null,
        ImageUrl: null,
        CategoryId: null,
        ExpenseReportId: null);

    private static UpdateReceiptDto SampleUpdateDto() => new(
        Merchant: "ICA Updated",
        Date: new DateTime(2026, 5, 1, 12, 0, 0, DateTimeKind.Utc),
        TotalAmount: 300m,
        VatAmount: 60m,
        Description: "Updated",
        ImageUrl: null,
        CategoryId: null,
        ExpenseReportId: null);

    [Fact]
    public async Task CreateAsync_ReturnsCreatedReceipt_WhenInputIsValid()
    {
        // Arrange — valid DTO + an authenticated user
        var repo = Substitute.For<IReceiptRepository>();
        var currentUser = Substitute.For<ICurrentUserService>();
        currentUser.UserId.Returns("user-123");
        var sut = new ReceiptService(repo, currentUser);

        // Act — create a new receipt
        var result = await sut.CreateAsync(SampleCreateDto());

        // Assert — response carries the right data
        result.ShouldNotBeNull();
        result.Merchant.ShouldBe("ICA");
        result.TotalAmount.ShouldBe(250m);
        result.UserId.ShouldBe("user-123");
    }

    [Fact]
    public async Task CreateAsync_SetsUserIdToCurrentUser()
    {
        // Arrange — current user is "user-abc"
        var repo = Substitute.For<IReceiptRepository>();
        var currentUser = Substitute.For<ICurrentUserService>();
        currentUser.UserId.Returns("user-abc");
        var sut = new ReceiptService(repo, currentUser);

        // Act — create a receipt
        await sut.CreateAsync(SampleCreateDto());

        // Assert — UserId on the entity passed to AddAsync came from CurrentUserService,
        // NOT from the DTO (which doesn't even carry one — that's the security win)
        await repo.Received(1).AddAsync(Arg.Is<Receipt>(r => r.UserId == "user-abc"));
    }

    [Fact]
    public async Task GetByIdAsync_ReturnsReceipt_WhenExists()
    {
        // Arrange — repo has a matching receipt
        var existing = new Receipt("ICA", DateTime.UtcNow, 100m, 20m, "user-1");
        var repo = Substitute.For<IReceiptRepository>();
        repo.GetByIdAsync(existing.Id).Returns(existing);
        var sut = new ReceiptService(repo, Substitute.For<ICurrentUserService>());

        // Act — fetch the receipt
        var result = await sut.GetByIdAsync(existing.Id);

        // Assert — response carries the entity's data
        result.Id.ShouldBe(existing.Id);
        result.Merchant.ShouldBe("ICA");
    }

    [Fact]
    public async Task GetByIdAsync_ThrowsNotFoundException_WhenMissing()
    {
        // Arrange — repo returns null for any id
        var repo = Substitute.For<IReceiptRepository>();
        repo.GetByIdAsync(Arg.Any<Guid>()).Returns((Receipt?)null);
        var sut = new ReceiptService(repo, Substitute.For<ICurrentUserService>());

        // Act — attempt to fetch a missing receipt
        Func<Task> act = async () => await sut.GetByIdAsync(Guid.NewGuid());

        // Assert — service throws domain exception
        await act.ShouldThrowAsync<NotFoundException>();
    }

    [Fact]
    public async Task UpdateAsync_ThrowsForbiddenException_WhenUserIsNotOwnerAndNotAdmin()
    {
        // Arrange — receipt owned by "owner", current user is someone else (not admin)
        var existing = new Receipt("ICA", DateTime.UtcNow, 100m, 20m, "owner");
        var repo = Substitute.For<IReceiptRepository>();
        repo.GetByIdAsync(existing.Id).Returns(existing);
        var currentUser = Substitute.For<ICurrentUserService>();
        currentUser.UserId.Returns("intruder");
        currentUser.IsAdmin.Returns(false);
        var sut = new ReceiptService(repo, currentUser);

        // Act — attempt to update someone else's receipt
        Func<Task> act = async () => await sut.UpdateAsync(existing.Id, SampleUpdateDto());

        // Assert — service blocks it
        await act.ShouldThrowAsync<ForbiddenException>();
    }

    [Fact]
    public async Task UpdateAsync_Succeeds_WhenUserIsAdmin()
    {
        // Arrange — admin updating someone else's receipt
        var existing = new Receipt("ICA", DateTime.UtcNow, 100m, 20m, "owner");
        var repo = Substitute.For<IReceiptRepository>();
        repo.GetByIdAsync(existing.Id).Returns(existing);
        var currentUser = Substitute.For<ICurrentUserService>();
        currentUser.UserId.Returns("admin-user");
        currentUser.IsAdmin.Returns(true);
        var sut = new ReceiptService(repo, currentUser);

        // Act — admin performs the update
        var result = await sut.UpdateAsync(existing.Id, SampleUpdateDto());

        // Assert — update went through, repo was asked to persist it
        result.Merchant.ShouldBe("ICA Updated");
        result.TotalAmount.ShouldBe(300m);
        repo.Received(1).Update(Arg.Any<Receipt>());
    }

    [Fact]
    public async Task DeleteAsync_CallsRepositoryDelete_WhenAuthorized()
    {
        // Arrange — owner deleting their own receipt
        var existing = new Receipt("ICA", DateTime.UtcNow, 100m, 20m, "owner-user");
        var repo = Substitute.For<IReceiptRepository>();
        repo.GetByIdAsync(existing.Id).Returns(existing);
        var currentUser = Substitute.For<ICurrentUserService>();
        currentUser.UserId.Returns("owner-user");
        currentUser.IsAdmin.Returns(false);
        var sut = new ReceiptService(repo, currentUser);

        // Act — delete it
        await sut.DeleteAsync(existing.Id);

        // Assert — repo.Remove was invoked once with the actual receipt
        repo.Received(1).Remove(existing);
    }
}