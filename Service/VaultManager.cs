using System;
using System.Collections.Generic;
using System.Linq;

using BitwardenVaultManager.DataAccess;
using BitwardenVaultManager.Service.Mapping;
using BitwardenVaultManager.Service.Models;

namespace BitwardenVaultManager.Service
{
    public class VaultManager
    {
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
                    errors.Add($"The '{item.Name}' login does not have ane 'Email Address' field");
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
        
        public IEnumerable<string> GetWeakPasswords()
            => GetPasswords().Where(password => passwordChecker.GetPasswordStrength(password) < PasswordStrength.Strong);

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
    }
}
