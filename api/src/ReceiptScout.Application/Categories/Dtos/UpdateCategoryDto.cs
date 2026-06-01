using System;
using System.Collections.Generic;
using System.Text;

namespace ReceiptScout.Application.Categories.Dtos
{
    public record UpdateCategoryDto(string Name, string BasAccount, decimal VatRate);

}
