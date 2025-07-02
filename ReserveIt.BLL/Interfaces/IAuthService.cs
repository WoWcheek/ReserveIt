using ReserveIt.BLL.DTOs;

namespace ReserveIt.BLL.Interfaces;

public interface IAuthService
{
    Task<string> Login(LoginDTO loginDto);
    Task<bool> Register(LoginDTO registerDto);
}
