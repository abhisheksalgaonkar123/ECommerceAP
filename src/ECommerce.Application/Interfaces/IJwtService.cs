using ECommerce.Application.DTOs.Auth;
using ECommerce.Domain.Entities;

namespace ECommerce.Application.Interfaces;

public interface IJwtService
{
    AuthResponseDto GenerateToken(ApplicationUser user);
    string GetUserIdFromToken(string token);
}
