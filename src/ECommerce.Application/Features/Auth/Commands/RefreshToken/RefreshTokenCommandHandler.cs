using ECommerce.Application.Common.Exceptions;
using ECommerce.Application.DTOs.Auth;
using ECommerce.Application.Interfaces;
using ECommerce.Domain.Entities;
using ECommerce.Domain.Interfaces;
using FluentValidation;
using MediatR;

namespace ECommerce.Application.Features.Auth.Commands.RefreshToken;

public class RefreshTokenCommandHandler:IRequestHandler<RefreshTokenCommand,AuthResponseDto>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IJwtService _jwtService;

    public RefreshTokenCommandHandler(
        IUnitOfWork unitOfWork,
        IJwtService jwtService)
    {
        _unitOfWork = unitOfWork;
        _jwtService = jwtService;
    }
    public async Task<AuthResponseDto> Handle(RefreshTokenCommand request, CancellationToken cancellationToken)
    {
        var users = await _unitOfWork.Repository<ApplicationUser>().FindAsync(u => u.RefreshToken == request.Token,cancellationToken);
        var user = users.FirstOrDefault();
        if (user is null)
            throw new NotFoundException(nameof(ApplicationUser),request.Token);
        if (user.RefreshTokenExpiry < DateTime.UtcNow)
        {
            throw new ValidationException("Refresh token expired");
        }

        var response = _jwtService.GenerateToken(user);
        // 4. Save new refresh token
        user.SetRefreshToken(
            response.RefreshToken,
            response.ExpiresAt
        );

        // 5. Save changes
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        // 6. Return response
        return response;
    }
}
