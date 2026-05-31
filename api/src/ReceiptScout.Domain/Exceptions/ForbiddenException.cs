using System;
using System.Collections.Generic;
using System.Text;

namespace ReceiptScout.Application.Common.Exceptions;

public class ForbiddenException : Exception
{
    public ForbiddenException(string message = "You are not authorized to perform this action.")
        : base(message) { }
}
