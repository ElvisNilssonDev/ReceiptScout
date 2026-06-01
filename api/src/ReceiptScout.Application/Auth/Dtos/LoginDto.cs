using System;
using System.Collections.Generic;
using System.Text;

namespace ReceiptScout.Application.Auth.Dtos;

public record LoginDto(string Email, string Password);
