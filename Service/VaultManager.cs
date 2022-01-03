using System;
using System.Collections.Generic;
using System.Linq;

using BitwardenVaultManager.DataAccess;
using BitwardenVaultManager.Service.Mapping;
using BitwardenVaultManager.Service.Models;

namespace BitwardenVaultManager.Service
{
    public class VaultManager : IVaultManager
    {
        static string WeakPasswordFieldName => "Weak Password";

        readonly IBitwardenVaultFileHandler vaultFileHandler;
        readonly IPasswordChecker passwordChecker;

        BitwardenVault vault;

        public VaultManager()
            : this(
                new BitwardenVaultFileHandler(),
                new PasswordChecker())
        {
            
        }

        public VaultManager(
            IBitwardenVaultFileHandler vaultFileHandler,
            IPasswordChecker passwordChecker)
        {
            this.vaultFileHandler = vaultFileHandler;
            this.passwordChecker = passwordChecker;
        }

        public void Load(string filePath)
        {
            vault = vaultFileHandler.Load(filePath).ToServiceModel();
        }

        public IEnumerable<string> GetMisconfiguredItems()
        {
            IList<string> errors = new List<string>();

            foreach (BitwardenItem item in vault.Items.Where(x => x.Type == BitwardenItemType.Login))
            {
                if (string.IsNullOrWhiteSpace(item.EmailAddress))
                {
                    errors.Add($"The '{item.Name}' login does not have an 'Email Address' field");
                }
            }

            return errors;
        }

        public string GetFolderName(Guid folderId)
            => vault.Folders.FirstOrDefault(folder => folder.Id.Equals(folderId))?.Name;

        public IEnumerable<string> GetEmailAddresses()
            => vault.Items
                .Where(item => !string.IsNullOrWhiteSpace(item.EmailAddress))
                .Select(item => item.EmailAddress.ToLowerInvariant())
                .Distinct();

        public IEnumerable<string> GetPasswords()
            => vault.Items
                .Where(item =>
                    item.Type == BitwardenItemType.Login &&
                    !(string.IsNullOrWhiteSpace(item.Login.Password)))
                .Select(item => item.Login.Password)
                .Distinct();

        public IEnumerable<BitwardenItem> GetItemsByEmailAddress(string emailAddress)
            => vault.Items
                .Where(item =>
                    !string.IsNullOrWhiteSpace(item.EmailAddress) &&
                    item.EmailAddress.Equals(emailAddress, StringComparison.InvariantCultureIgnoreCase));

        public IEnumerable<BitwardenItem> GetItemsByPassword(string password)
            => vault.Items
                .Where(item =>
                    item.Type == BitwardenItemType.Login &&
                    !(string.IsNullOrWhiteSpace(item.Login.Password)) &&
                    item.Login.Password.Equals(password, StringComparison.InvariantCulture));

        public IEnumerable<BitwardenItem> GetItemsByPasswordContaining(string text)
            => vault.Items
                .Where(item =>
                    item.Type == BitwardenItemType.Login &&
                    !(string.IsNullOrWhiteSpace(item.Login.Password)) &&
                    item.Login.Password.Contains(text, StringComparison.InvariantCulture));

        public IEnumerable<BitwardenItem> GetItemsByPasswordLength(int length)
            => vault.Items
                .Where(item =>
                    item.Type == BitwardenItemType.Login &&
                    !(string.IsNullOrWhiteSpace(item.Login.Password)) &&
                    item.Login.Password.Length.Equals(length));

        public IEnumerable<BitwardenItem> GetItemsByUsername(string username)
            => vault.Items
                .Where(item =>
                    item.Type == BitwardenItemType.Login &&
                    !(string.IsNullOrWhiteSpace(item.Login.Username)) &&
                    item.Login.Username.Equals(username, StringComparison.InvariantCultureIgnoreCase));
        
        public IEnumerable<BitwardenItem> GetItemsWithWeakPasswords()
            => vault.Items
                .Where(item =>
                    item.Type == BitwardenItemType.Login &&
                    !(string.IsNullOrWhiteSpace(item.Login.Password)) &&
                    (
                        item.Fields is null ||
                        item.Fields.All(x => !x.Name.Equals(WeakPasswordFieldName)) ||
                        item.Fields.First(x => x.Name.Equals(WeakPasswordFieldName)).Value.Equals(false.ToString(), StringComparison.InvariantCultureIgnoreCase)
                    ))
                .Where(item => passwordChecker.GetPasswordStrength(item.Login.Password) < PasswordStrength.Strong);
        
        public IEnumerable<BitwardenItem> GetItemsWithoutTotp()
            => vault.Items
                .Where(item =>
                    item.Type == BitwardenItemType.Login &&
                    string.IsNullOrWhiteSpace(item.Login.TOTP))
                .Where(item => passwordChecker.GetPasswordStrength(item.Login.Password) < PasswordStrength.Strong);

        public IEnumerable<string> GetTotpUrls()
            => vault.Items
                .Where(item =>
                    item.Type == BitwardenItemType.Login &&
                    !(string.IsNullOrWhiteSpace(item.Login.TOTP)))
                .OrderBy(item => item.IsFavourite)
                .ThenBy(item => item.Name)
                .ThenBy(item => item.Username)
                .Select(item => GetTotpUrl(item));


        string GetTotpUrl(BitwardenItem item)
        {
            string method = "totp";
            int digits = 6;
            int period = 30;

            if (item.Name.Contains("Gemini", StringComparison.InvariantCultureIgnoreCase))
            {
                digits = 7;
                period = 10;
            }
            else if (item.Name.Contains("Battle.net", StringComparison.InvariantCultureIgnoreCase) ||
                     item.Name.Contains("Blizzard", StringComparison.InvariantCultureIgnoreCase))
            {
                digits = 8;
            }
            else if (item.Name.Contains("Steam", StringComparison.InvariantCultureIgnoreCase))
            {
                method = "steam";
                digits = 5;
            }

            string rawUrl = $"otpauth://{method}/{item.Name}:{item.Username}:?secret={item.Login.TOTP}&digits={digits}&period={period}&issuer={item.Name}";
            
            return Uri.EscapeUriString(rawUrl);
        }
    }
}
