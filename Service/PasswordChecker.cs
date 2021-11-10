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

            if (password.Any(c => char.IsUpper(c)) &&
                password.Any(c => char.IsLower(c)))
            {
                strength += 1;
            }
            if (password.Where(c => char.IsUpper(c)).Count() >= RecommendedUppercaseLettersCount &&
                password.Where(c => char.IsLower(c)).Count() >= RecommendedLowercaseLettersCount)
            {
                strength += 1;
            }

            if (password.Any(c => char.IsDigit(c)))
            {
                strength += 1;
            }
            if (password.Where(c => char.IsDigit(c)).Count() >= RecommendedDigitsCount)
            {
                strength += 1;
            }

            if (password.Any(c => "!@#$%^&*?_~-£().,".Contains(c)))
            {
                strength += 1;
            }
            if (password.Where(c => "!@#$%^&*?_~-£().,".Contains(c)).Count() >= RecommendedSymbolsCount)
            {
                strength += 1;
            }

            return (PasswordStrength)Math.Min(strength, (int)PasswordStrength.Ultimate);
        }
    }
}
