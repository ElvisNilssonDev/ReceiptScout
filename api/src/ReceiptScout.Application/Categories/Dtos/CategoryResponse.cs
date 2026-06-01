using System;
using System.Collections.Generic;
using System.Text;

namespace ReceiptScout.Application.Categories.Dtos
{
    public record CategoryResponse(
    Guid Id, string Name, string Slug, string BasAccount, decimal VatRate);

}
