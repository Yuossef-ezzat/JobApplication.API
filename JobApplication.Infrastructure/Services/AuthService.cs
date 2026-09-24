using JobApplication.Application.Abstractions.ResultPattern;
using JobApplication.Application.DTOs;
using JobApplication.Application.Interfaces;
using JobApplication.Application.Interfaces.IServices;
using JobApplication.Domain.Entities;
using JobApplication.Domain.Enums;
using JobApplication.Infrastructure.Identity;
using Microsoft.AspNetCore.Identity;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace JobApplication.Infrastructure.Services
{
    public class AuthService : IAuthService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ITokenService _tokenService;
        private readonly IUnitOfWork _unitOfWork;

        public AuthService(
            UserManager<ApplicationUser> userManager,
            ITokenService tokenService,
            IUnitOfWork unitOfWork)
        {
            _userManager = userManager;
            _tokenService = tokenService;
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<AuthResponse>> RegisterAsync(RegisterRequest request)
        {
            var existingUser = await _userManager.FindByEmailAsync(request.Email);
            if (existingUser != null)
                return Result<AuthResponse>.Failure(new Error(400, "A user with this email already exists."));

            await _unitOfWork.BeginTransactionAsync();
            try
            {
                var user = new ApplicationUser
                {
                    UserName = request.Email,
                    Email = request.Email,
                    FullName = request.FullName,
                    UserType = request.UserType
                };

                var result = await _userManager.CreateAsync(user, request.Password);
                if (!result.Succeeded)
                {
                    await _unitOfWork.RollbackTransactionAsync();
                    var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                    return Result<AuthResponse>.Failure(new Error(400, errors));
                }

                var roleName = request.UserType.ToString();
                var roleResult = await _userManager.AddToRoleAsync(user, roleName);
                if (!roleResult.Succeeded)
                {
                    await _unitOfWork.RollbackTransactionAsync();
                    var errors = string.Join(", ", roleResult.Errors.Select(e => e.Description));
                    return Result<AuthResponse>.Failure(new Error(400, errors));
                }

                if (request.UserType == UserType.Recruiter)
                {
                    var recruiter = new Recruiter
                    {
                        ApplicationUserId = user.Id,
                        CompanyName = !string.IsNullOrWhiteSpace(request.CompanyName)
                            ? request.CompanyName
                            : request.FullName
                    };
                    await _unitOfWork.Recruiters.InsertAsync(recruiter);
                }
                else if (request.UserType == UserType.Candidate)
                {
                    var candidate = new Candidate
                    {
                        ApplicationUserId = user.Id,
                        Name = request.FullName,
                        CvUrl = string.Empty
                    };
                    await _unitOfWork.Candidates.InsertAsync(candidate);
                }

                await _unitOfWork.CommitTransactionAsync();

                var token = _tokenService.GenerateAccessToken(user.Id, user.Email!, roleName);

                return Result<AuthResponse>.Success(new AuthResponse
                {
                    Token = token.Value,
                    Email = user.Email!,
                    Role = roleName
                });
            }
            catch
            {
                await _unitOfWork.RollbackTransactionAsync();
                throw;
            }
        }

        public async Task<Result<AuthResponse>> LoginAsync(LoginRequest request)
        {
            var user = await _userManager.FindByEmailAsync(request.Email);
            if (user == null)
                return Result<AuthResponse>.Failure(new Error(401, "Invalid email or password."));

            var validPassword = await _userManager.CheckPasswordAsync(user, request.Password);
            if (!validPassword)
                return Result<AuthResponse>.Failure(new Error(401, "Invalid email or password."));

            var roles = await _userManager.GetRolesAsync(user);
            var role = roles.FirstOrDefault() ?? user.UserType.ToString();

            var token = _tokenService.GenerateAccessToken(user.Id, user.Email!, role);

            return Result<AuthResponse>.Success(new AuthResponse
            {
                Token = token.Value,
                Email = user.Email!,
                Role = role
            });
        }
    }
}
