using System;
using System.Collections.Generic;
using System.Text;

namespace ReceiptScout.Application.Auth.Dtos;

public record AuthResponse(
    string AccessToken,
    string Email,
    IReadOnlyList<string> Roles);