using JobApplication.Application.Abstractions.ResultPattern;
using JobApplication.Application.DTOs;
using System.Threading.Tasks;

namespace JobApplication.Application.Interfaces.IServices
{
    public interface IAuthService
    {
        Task<Result<AuthResponse>> RegisterAsync(RegisterRequest request);
        Task<Result<AuthResponse>> LoginAsync(LoginRequest request);
    }
}
