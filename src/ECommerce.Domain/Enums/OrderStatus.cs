namespace ECommerce.Domain.Enums;

public enum OrderStatus
{
    Pending     = 1,  // order placed, awaiting confirmation
    Confirmed   = 2,  // shop confirmed the order
    Processing  = 3,  // being prepared
    Shipped     = 4,  // on the way to customer
    Delivered   = 5,  // received by customer
    Cancelled   = 6   // cancelled
}