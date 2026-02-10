namespace Ordering.Domain.Enums;

public enum OrderStatus
{
    Created = 0,
    InPreparation = 1,
    Ready = 2,
    Served = 3,
    Paid = 4,
    Cancelled = 5
}