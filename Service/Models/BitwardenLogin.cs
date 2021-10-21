namespace BitwardenVaultManager.Service.Models
{
    public sealed class BitwardenLogin
    {
        public string Username { get; set; }

        public string Password { get; set; }

        public string TOTP { get; set; }
    }
}
