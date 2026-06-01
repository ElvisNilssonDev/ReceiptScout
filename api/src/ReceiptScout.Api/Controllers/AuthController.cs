using Microsoft.AspNetCore.Mvc;
using ReceiptScout.Application.Auth;
using ReceiptScout.Application.Auth.Dtos;

namespace ReceiptScout.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _auth;

    public AuthController(IAuthService auth) => _auth = auth;

    [HttpPost("register")]
    public async Task<ActionResult<AuthResponse>> Register(RegisterDto dto)
        => Ok(await _auth.RegisterAsync(dto));

    [HttpPost("login")]
    public async Task<ActionResult<AuthResponse>> Login(LoginDto dto)
        => Ok(await _auth.LoginAsync(dto));
}