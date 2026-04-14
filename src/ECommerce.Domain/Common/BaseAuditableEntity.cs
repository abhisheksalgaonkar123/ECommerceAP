using System;

namespace ECommerce.Domain.Common;

public abstract class BaseAuditableEntity : BaseEntity
{
    public DateTime CreatedAt { get; set; }

    // Who created it? (will be the logged-in user's email/id)
    public string? CreatedBy { get; set; }

    // When was it last changed?
    public DateTime? LastModifiedAt { get; set; }

    // Who last changed it?
    public string? LastModifiedBy { get; set; }


}
