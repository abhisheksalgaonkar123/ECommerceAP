using MediatR;

namespace ECommerce.Domain.Common;

// INotification is from MediatR library
// It marks this class as something that can be "published"
// and "handled" by other parts of the system
public abstract class BaseEvent : INotification { }