using JobApplication.Application.Abstractions.ResultPattern;

namespace JobApplication.Application.Interfaces.IServices
{
    public interface ITokenService
    {
        Result<string> GenerateAccessToken(string userId, string email, string role);
    }
}
