using Microsoft.AspNetCore.Mvc;
using ReserveIt.BLL.DTOs;
using ReserveIt.BLL.Interfaces;

namespace ReserveIt.Presentation.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register(LoginDTO registerDto)
    {
        await _authService.Register(registerDto);
        return Ok(new { message = "User registered successfully" });
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginDTO loginDto)
    {
        var token = await _authService.Login(loginDto);
        return Ok(new { token });
    }
}
