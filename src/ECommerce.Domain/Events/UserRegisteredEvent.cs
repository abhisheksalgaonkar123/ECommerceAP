using ECommerce.Domain.Common;
using ECommerce.Domain.Entities;

namespace ECommerce.Domain.Events;

public sealed class UserRegisteredEvent : BaseEvent
{
    public ApplicationUser User { get; } // public not private!
    public UserRegisteredEvent(ApplicationUser user) => User = user;
}