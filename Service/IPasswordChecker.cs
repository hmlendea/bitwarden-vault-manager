namespace BitwardenVaultManager.Service
{
    public interface IPasswordChecker
    {
        PasswordStrength GetPasswordStrength(string password);
    }
}
