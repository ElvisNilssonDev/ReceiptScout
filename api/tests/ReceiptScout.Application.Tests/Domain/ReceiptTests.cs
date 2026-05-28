using ReceiptScout.Domain.Entities;
using Shouldly;
using Xunit;

namespace ReceiptScout.Application.Tests.Domain;

public class ReceiptTests
{

    [Fact]
    public void Receipt_COnstructor_ThrowsArgumentException_WhenAmountIsNegative()
    {
        var act = () => new Receipt(
            merchant: "ICA",
            date: DateTime.UtcNow,
            totalAmount: -100m,
            vatAmount: 25m,
            userId: "user-123");

        Should.Throw<ArgumentException>(act);
    }

    [Fact]
    public void Receipt_Constructor_ThrowsArgumentException_WhenMerchantIsEmpty()
    {
        var act = () => new Receipt(
            merchant: "",
            date: DateTime.UtcNow,
            totalAmount: 100m,
            vatAmount: 25m,
            userId: "user-123");

        Should.Throw<ArgumentException>(act);
    }
}