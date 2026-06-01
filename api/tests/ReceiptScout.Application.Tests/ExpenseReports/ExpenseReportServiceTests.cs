using NSubstitute;
using ReceiptScout.Application.Common.Exceptions;
using ReceiptScout.Application.Common.Interfaces;
using ReceiptScout.Application.ExpenseReports;
using ReceiptScout.Domain.Entities;
using ReceiptScout.Domain.Enums;
using Shouldly;
using Xunit;

namespace ReceiptScout.Application.Tests.ExpenseReports;

public class ExpenseReportServiceTests
{
    [Fact]
    public async Task SubmitAsync_ChangesStatusToSubmitted()
    {
        // Arrange
        var existing = new ExpenseReport("Q1 expenses", "owner-1");
        var repo = Substitute.For<IExpenseReportRepository>();
        repo.GetByIdAsync(existing.Id).Returns(existing);
        var currentUser = Substitute.For<ICurrentUserService>();
        currentUser.UserId.Returns("owner-1");
        var sut = new ExpenseReportService(repo, currentUser);

        // Act
        var result = await sut.SubmitAsync(existing.Id);

        // Assert
        result.Status.ShouldBe(ExpenseReportStatus.Submitted);
        result.SubmittedAt.ShouldNotBeNull();
    }

    [Fact]
    public async Task ApproveAsync_ThrowsForbiddenException_WhenUserIsNotAdmin()
    {
        // Arrange
        var existing = new ExpenseReport("Q1 expenses", "owner-1");
        existing.Submit();
        var repo = Substitute.For<IExpenseReportRepository>();
        repo.GetByIdAsync(existing.Id).Returns(existing);
        var currentUser = Substitute.For<ICurrentUserService>();
        currentUser.UserId.Returns("regular-user");
        currentUser.IsAdmin.Returns(false);
        var sut = new ExpenseReportService(repo, currentUser);

        // Act
        Func<Task> act = async () => await sut.ApproveAsync(existing.Id);

        // Assert
        await act.ShouldThrowAsync<ForbiddenException>();
    }
}