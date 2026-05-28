using System;
using Microsoft.AspNetCore.Identity;
using ReceiptScout.Domain.Entities;
using System.Collections.Generic;
using System.Text;

namespace ReceiptScout.Domain.Identity
{
    public class ApplicationUser : IdentityUser
    {
        public ICollection<Receipt> Receipt { get; private set; } = new List<Receipt>();
        public ICollection<ExpenseReport> ExpenseReports { get; private set; } = new List<ExpenseReport>();
    }
}

