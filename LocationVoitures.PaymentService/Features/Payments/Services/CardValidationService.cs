namespace LocationVoitures.PaymentService.Features.Payments.Services;

public class CardValidationService
{
    public string NormalizeCardNumber(string cardNumber)
    {
        return new string(cardNumber.Where(char.IsDigit).ToArray());
    }

    public bool IsValidLuhn(string cardNumber)
    {
        if (string.IsNullOrWhiteSpace(cardNumber))
        {
            return false;
        }

        var normalized = NormalizeCardNumber(cardNumber);

        if (normalized.Length is < 13 or > 19)
        {
            return false;
        }

        var sum = 0;
        var alternate = false;

        for (var index = normalized.Length - 1; index >= 0; index--)
        {
            var digit = normalized[index] - '0';

            if (alternate)
            {
                digit *= 2;

                if (digit > 9)
                {
                    digit -= 9;
                }
            }

            sum += digit;
            alternate = !alternate;
        }

        return sum % 10 == 0;
    }

    public bool IsExpired(int expirationMonth, int expirationYear)
    {
        var now = DateTime.UtcNow;
        return expirationYear < now.Year ||
               (expirationYear == now.Year && expirationMonth < now.Month);
    }
}
