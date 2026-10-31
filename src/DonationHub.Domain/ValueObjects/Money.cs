using DonationHub.Domain.Common;
using DonationHub.Domain.Exceptions;

namespace DonationHub.Domain.ValueObjects;

public class Money : ValueObject
{
    public const string DefaultCurrency = "USD";

    public decimal Amount { get; private set; }
    public string Currency { get; private set; } = DefaultCurrency;

    // Parameterless constructor for EF Core serialization/reflection
    private Money()
    {
    }

    private Money(decimal amount, string currency)
    {
        if (amount < 0)
            throw new DomainException("Money amount cannot be negative.");

        if (string.IsNullOrWhiteSpace(currency))
            throw new DomainException("Currency cannot be empty.");

        Amount = decimal.Round(amount, 2, MidpointRounding.AwayFromZero);
        Currency = currency.Trim().ToUpperInvariant();
    }

    public static Money From(decimal amount, string currency = DefaultCurrency)
    {
        return new Money(amount, currency);
    }

    public static Money Zero(string currency = DefaultCurrency)
    {
        return new Money(0m, currency);
    }

    public Money Add(Money other)
    {
        EnsureSameCurrency(other);
        return new Money(Amount + other.Amount, Currency);
    }

    public Money Subtract(Money other)
    {
        EnsureSameCurrency(other);
        if (Amount < other.Amount)
            throw new DomainException("Cannot subtract a larger amount from a smaller amount.");

        return new Money(Amount - other.Amount, Currency);
    }

    private void EnsureSameCurrency(Money other)
    {
        if (other is null)
            throw new ArgumentNullException(nameof(other));

        if (!string.Equals(Currency, other.Currency, StringComparison.OrdinalIgnoreCase))
            throw new DomainException($"Cannot operate on money with different currencies: '{Currency}' and '{other.Currency}'.");
    }

    public static Money operator +(Money left, Money right) => left.Add(right);
    public static Money operator -(Money left, Money right) => left.Subtract(right);

    public static bool operator <(Money left, Money right)
    {
        left.EnsureSameCurrency(right);
        return left.Amount < right.Amount;
    }

    public static bool operator >(Money left, Money right)
    {
        left.EnsureSameCurrency(right);
        return left.Amount > right.Amount;
    }

    public static bool operator <=(Money left, Money right)
    {
        left.EnsureSameCurrency(right);
        return left.Amount <= right.Amount;
    }

    public static bool operator >=(Money left, Money right)
    {
        left.EnsureSameCurrency(right);
        return left.Amount >= right.Amount;
    }

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Amount;
        yield return Currency;
    }

    public override string ToString() => $"{Amount:0.00} {Currency}";
}

