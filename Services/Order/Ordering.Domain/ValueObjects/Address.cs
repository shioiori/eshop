namespace Ordering.Domain.ValueObjects
{
    public record Address
    {
        public string FirstName { get; private set; }
        public string LastName { get; private set; }
        public string? EmailAddress { get; private set; }
        public string AddressLine { get; private set; }
        public string Country { get; private set; }
        public string State { get; private set; }
        public string Zipcode { get; private set; }
        protected Address(string firstName, string lastName, string emailAddress, string addressLine, string country, string state, string zipcode) 
        {
            FirstName = firstName;
            LastName = lastName;
            EmailAddress = emailAddress;
            AddressLine = addressLine;
            Country = country;
            State = state;
            Zipcode = zipcode;
        }
        public static Address Of(string firstName, string lastName, string emailAddress, string addressLine, string country, string state, string zipcode)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(emailAddress);
            ArgumentException.ThrowIfNullOrWhiteSpace(addressLine);
            return new Address(firstName, lastName, emailAddress, addressLine, country, state, zipcode);
        }
    }
}
