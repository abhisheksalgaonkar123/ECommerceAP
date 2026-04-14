using ECommerce.Application.DTOs.Auth;
using MediatR;

namespace ECommerce.Application.Features.Auth.Commands.Login;

public record LoginCommand(string Email, string Password) : IRequest<AuthResponseDto>;
