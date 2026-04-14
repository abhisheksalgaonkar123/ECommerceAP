using ECommerce.Application.DTOs.Auth;
using ECommerce.Application.Interfaces;
using ECommerce.Domain.Entities;
using ECommerce.Domain.Interfaces;
using FluentValidation;
using MediatR;

namespace ECommerce.Application.Features.Auth.Commands.Register;

public class RegisterCommandHandler
    : IRequestHandler<RegisterCommand, AuthResponseDto>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IJwtService _jwtService;

    public RegisterCommandHandler(
        IUnitOfWork unitOfWork,
        IJwtService jwtService)
    {
        _unitOfWork = unitOfWork;
        _jwtService = jwtService;
    }

    public async Task<AuthResponseDto> Handle(
        RegisterCommand request,
        CancellationToken cancellationToken)
    {
        // 1. Check email already exists
        var existingUsers = await _unitOfWork
            .Repository<ApplicationUser>()
            .FindAsync(u => u.Email == request.Email.ToLower(),
                cancellationToken);

        if (existingUsers.Any())
            throw new ValidationException("Email already registered.");

        // 2. Hash password — never store plain text!
        var passwordHash = BCrypt.Net.BCrypt
            .HashPassword(request.Password);

        // 3. Create user entity
        var user = ApplicationUser.Create(
            request.FirstName,   // ← correct order!
            request.LastName,
            request.Email,
            passwordHash);

        // 4. Save user
        await _unitOfWork
            .Repository<ApplicationUser>()
            .AddAsync(user, cancellationToken);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        // 5. Generate JWT + RefreshToken
        var response = _jwtService.GenerateToken(user);

        // 6. Save refresh token on user
        user.SetRefreshToken(
            response.RefreshToken,  // ← RefreshToken not Token!
            response.ExpiresAt);

        // 7. Save refresh token to database
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return response;
    }
}
