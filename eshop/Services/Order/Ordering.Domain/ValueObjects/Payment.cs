namespace Ordering.Domain.ValueObjects
{
    public record Payment(string CardName, string CardNumber, string Expiration, string CVV, string PaymentMethod)
    {
        public static Payment Of(string cardName, string cardNumber, string expiration, string cvv, string paymentMethod)
        {
            return new Payment(cardName, cardNumber, expiration, cvv, paymentMethod);
        }
    }
}
