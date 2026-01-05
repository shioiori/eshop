namespace Ordering.Domain.ValueObjects
{
    public record CustomerId
    {
        public Guid Value { get; private set; }
        public static CustomerId Of(Guid value)
        {
            ArgumentNullException.ThrowIfNull(value);
            return new CustomerId { Value = value };
        }
    }
}
