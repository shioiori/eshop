namespace Ordering.Domain.ValueObjects
{
    public record ProductId
    {
        public Guid Value { get; private set; }
        public static ProductId Of(Guid value)
        {
            ArgumentNullException.ThrowIfNull(value);
            return new ProductId { Value = value };
        }
    }
}
