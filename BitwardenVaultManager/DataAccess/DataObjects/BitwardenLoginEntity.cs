namespace BitwardenVaultManager.DataAccess.DataObjects
{
    public sealed class BitwardenLoginEntity
    {
        public string Username { get; set; }

        public string Password { get; set; }

        public string TOTP { get; set; }
    }
}
