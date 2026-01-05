namespace Ordering.Domain.ValueObjects
{
    public record Payment
    {
        public string? CardName { get; }
        public string CardNumber { get; }
        public string Expiration { get; }
        public string CVV { get; }
        public string PaymentMethod { get; }
        protected Payment(string cardName, string cardNumber, string expiration, string cvv, string paymentMethod)
        {
            CardName = cardName;
            CardNumber = cardNumber;
            Expiration = expiration;
            CVV = cvv;
            PaymentMethod = paymentMethod;
        }

        public static Payment Create(string cardName, string cardNumber, string expiration, string cvv, string paymentMethod)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(cardNumber);
            ArgumentException.ThrowIfNullOrWhiteSpace(expiration);
            ArgumentException.ThrowIfNullOrWhiteSpace(cvv);
            ArgumentOutOfRangeException.ThrowIfGreaterThan(cvv.Length, 3);
            return new Payment(cardName, cardNumber, expiration, cvv, paymentMethod);
        }
    }
}
