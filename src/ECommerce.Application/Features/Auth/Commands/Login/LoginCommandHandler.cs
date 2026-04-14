using ECommerce.Application.Common.Exceptions;
using ECommerce.Application.DTOs.Auth;
using ECommerce.Application.Interfaces;
using ECommerce.Domain.Entities;
using ECommerce.Domain.Interfaces;
using FluentValidation;
using MediatR;

namespace ECommerce.Application.Features.Auth.Commands.Login;

public class LoginCommandHandler:IRequestHandler<LoginCommand,AuthResponseDto>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IJwtService _jwtService;

    public LoginCommandHandler(
        IUnitOfWork unitOfWork,
        IJwtService jwtService)
    {
        _unitOfWork = unitOfWork;
        _jwtService = jwtService;
    }
    public async Task<AuthResponseDto> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        var normalizedEmail = request.Email.ToLower().Trim();
        var userstest = await _unitOfWork
            .Repository<ApplicationUser>().GetAllAsync();
        var users = await _unitOfWork
            .Repository<ApplicationUser>()
            .FindAsync(u => u.Email == normalizedEmail,
                cancellationToken);
        var user = users.FirstOrDefault();

        if (user is null)
            throw new NotFoundException(nameof(ApplicationUser),request.Email);
        var isPasswordValid = BCrypt.Net.BCrypt.Verify(
            request.Password,
            user.PasswordHash);
        if (!isPasswordValid)
        {
            throw new ValidationException("Invalid email or password.");
        }
        var response = _jwtService.GenerateToken(user);

        user.SetRefreshToken(
            response.RefreshToken,
            response.ExpiresAt
        );

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return response;

    }
}
