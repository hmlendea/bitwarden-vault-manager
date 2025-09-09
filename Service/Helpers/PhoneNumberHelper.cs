using System;
using System.Linq;

namespace BitwardenVaultManager.Service.Helpers
{
    public static class PhoneNumberHelper
    {
        public static bool IsValid(string value)
            => !string.IsNullOrWhiteSpace(value) &&
               value.All(c => char.IsDigit(c) || c == '+' || c == '-' || c == ' ' || c == '(' || c == ')') &&
               value.Length >= 9;

        public static string Normalise(string phoneNumber)
        {
            string normalisedPhoneNumber = phoneNumber.Trim();

            normalisedPhoneNumber = normalisedPhoneNumber.Replace(" ", string.Empty);
            normalisedPhoneNumber = normalisedPhoneNumber.Replace("-", string.Empty);
            normalisedPhoneNumber = normalisedPhoneNumber.Replace("(", string.Empty);
            normalisedPhoneNumber = normalisedPhoneNumber.Replace(")", string.Empty);

            if (normalisedPhoneNumber.StartsWith("00", StringComparison.InvariantCulture))
            {
                normalisedPhoneNumber = "+" + normalisedPhoneNumber[2..];
            }
            else if (normalisedPhoneNumber.StartsWith("07", StringComparison.InvariantCulture))
            {
                normalisedPhoneNumber = "+40" + normalisedPhoneNumber[1..];
            }

            return normalisedPhoneNumber;
        }
    }
}
