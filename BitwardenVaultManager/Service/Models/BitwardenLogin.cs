namespace BitwardenVaultManager.Service.Models
{
    public sealed class BitwardenLogin
    {
        string totp;

        public string Username { get; set; }

        public string Password { get; set; }

        public string TOTP
        {
            get => totp?.Replace(" ", "");
            set => totp = value;
        }
    }
}
