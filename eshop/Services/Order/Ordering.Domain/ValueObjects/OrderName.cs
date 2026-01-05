namespace Ordering.Domain.ValueObjects
{
    public record OrderName
    {
        private const int DefaultLength = 5;
        public string Value { get; private set; }
        public static OrderName Of(string value)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(value);
            ArgumentOutOfRangeException.ThrowIfLessThan(value.Length, DefaultLength);
            return new OrderName { Value = value };
        }
    }
}
