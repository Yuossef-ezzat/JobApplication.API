using JobApplication.Application.Abstractions.ResultPattern;
using JobApplication.Application.Interfaces;
using JobApplication.Application.Interfaces.IServices;
using JobApplication.Domain.Entities;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace JobApplication.Infrastructure.Services
{
    public class TokenService : ITokenService
    {
        private readonly IConfiguration _configuration;
        private readonly IUnitOfWork _unitOfWork;

        public TokenService(IConfiguration configuration, IUnitOfWork unitOfWork )
        {
            _configuration = configuration;
            _unitOfWork = unitOfWork;
        }

        public Result<string> GenerateAccessToken(string userId, string email, string role)
        {
            var jwtSettings = _configuration.GetSection("JwtOptions");
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings["Key"]!));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, userId),
                new Claim(ClaimTypes.Email, email),
                new Claim(ClaimTypes.Role, role)
            };

            var expiryMinutes = int.Parse(jwtSettings["ExpiryMinutes"] ?? "60");

            var token = new JwtSecurityToken(
                issuer: jwtSettings["Issuer"],
                audience: jwtSettings["Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(expiryMinutes),
                signingCredentials: credentials
            );

            return Result<string>.Success(new JwtSecurityTokenHandler().WriteToken(token));
        }
        public async Task<Result<string>> GenerateRefreshToken(string userId)
        {
            var tokenExists = await _unitOfWork.Tokens.FindAsync(rt => rt.UserId == userId && rt.ExpirationDate > DateTime.UtcNow && rt.RevokedAt == null);
            if (tokenExists != null)
            {
                tokenExists.RevokedAt = DateTime.UtcNow;
                await _unitOfWork.SaveChangesAsync();
            }
            var tokenBytes = new byte[64];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(tokenBytes);
            var token = Convert.ToBase64String(tokenBytes);
            var TokenHash = Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(token)));
            var refreshToken = new RefreshToken
            {
                UserId = userId,
                HashToken = TokenHash,
                IssuedAt = DateTime.UtcNow,
                ExpirationDate = DateTime.UtcNow.AddDays(int.Parse(_configuration.GetSection("JwtOptions")["RefreshTokenExpiryDays"] ?? "7"))
            };
            await _unitOfWork.Tokens.InsertAsync(refreshToken);
            await _unitOfWork.SaveChangesAsync();

            return Result<string>.Success(token);
        }
    }
}
