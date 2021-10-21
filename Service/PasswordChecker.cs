using System;
using System.Linq;
 
namespace BitwardenVaultManager.Service
{
    public sealed class PasswordChecker : IPasswordChecker
    {
        public PasswordStrength GetPasswordStrength(string password)
        {
            int strength = 0;

            if (String.IsNullOrWhiteSpace(password))
            {
                return PasswordStrength.Terrible;
            }

            if (password.Length >= 8)
            {
                strength += 1;
            }

            if (password.Length >= 16)
            {
                strength += 1;
            }

            if (password.Length >= 32)
            {
                strength += 1;
            }

            if (password.Any(c => char.IsUpper(c)) &&
                password.Any(c => char.IsLower(c)))
            {
                strength += 1;
            }

            if (password.Any(c => char.IsDigit(c)))
            {
                strength += 1;
            }

            if (password.Any(c => "!@#$%^&*?_~-£().,".Contains(c)))
            {
                strength += 1;
            }

            return (PasswordStrength)Math.Min(strength, (int)PasswordStrength.VeryStrong);
        }
    }
}
