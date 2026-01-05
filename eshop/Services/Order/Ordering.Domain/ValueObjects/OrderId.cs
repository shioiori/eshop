namespace Ordering.Domain.ValueObjects
{
    public record OrderId
    {
        public Guid Value { get; private set; }
        public static OrderId Of(Guid value)
        {
            ArgumentNullException.ThrowIfNull(value);
            return new OrderId { Value = value };
        }
    }
}
