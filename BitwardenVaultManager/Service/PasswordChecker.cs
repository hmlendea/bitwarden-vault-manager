using System;
using System.Linq;

namespace BitwardenVaultManager.Service
{
    public sealed class PasswordChecker : IPasswordChecker
    {
        const int RecommendedSymbolsCount = 3;
        const int RecommendedUppercaseLettersCount = 3;
        const int RecommendedLowercaseLettersCount = 3;
        const int RecommendedDigitsCount = 3;

        public PasswordStrength GetPasswordStrength(string password)
        {
            int strength = 1;

            if (string.IsNullOrWhiteSpace(password))
            {
                return PasswordStrength.Terrible;
            }

            if (password.Length >= 10)
            {
                strength += 1;
            }

            if (password.Length >= 20)
            {
                strength += 1;
            }

            if (password.Length >= 30)
            {
                strength += 1;
            }

            if (password.Length >= 40)
            {
                strength += 1;
            }

            if (password.Length >= 50)
            {
                strength += 1;
            }

            if (password.Length >= 60)
            {
                strength += 1;
            }

            if (password.Any(char.IsUpper) &&
                password.Any(char.IsLower))
            {
                strength += 1;
            }
            if (password.Where(char.IsUpper).Count() >= RecommendedUppercaseLettersCount &&
                password.Where(char.IsLower).Count() >= RecommendedLowercaseLettersCount)
            {
                strength += 1;
            }

            if (password.Any(char.IsDigit))
            {
                strength += 1;
            }
            if (password.Where(char.IsDigit).Count() >= RecommendedDigitsCount)
            {
                strength += 1;
            }

            if (password.Any("!@#$%^&*?_~-£().,".Contains))
            {
                strength += 1;
            }
            if (password.Where("!@#$%^&*?_~-£().,".Contains).Count() >= RecommendedSymbolsCount)
            {
                strength += 1;
            }

            return (PasswordStrength)Math.Min(strength, (int)PasswordStrength.Ultimate);
        }
    }
}
