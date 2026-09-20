using JobApplication.Application.DTOs;
using System.Threading.Tasks;

namespace JobApplication.Application.Interfaces
{
    public interface IAuthService
    {
        Task<AuthResponse> RegisterAsync(RegisterRequest request);
        Task<AuthResponse> LoginAsync(LoginRequest request);
    }
}
