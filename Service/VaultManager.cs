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

        BitwardenVault vault;

        public VaultManager()
            : this(new BitwardenVaultFileHandler())
        {
            
        }

        public VaultManager(IBitwardenVaultFileHandler vaultFileHandler)
        {
            this.vaultFileHandler = vaultFileHandler;
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
            => vault.Folders
                .FirstOrDefault(folder => folder.Id.Equals(folderId))
                .Name;

        public IEnumerable<string> GetEmailAddresses()
            => vault.Items
                .Where(item => !string.IsNullOrWhiteSpace(item.EmailAddress))
                .Select(item => item.EmailAddress.ToLowerInvariant())
                .Distinct();

        public IEnumerable<BitwardenItem> GetItemsByEmailAddress(string emailAddress)
            => vault.Items
                .Where(item =>
                    !string.IsNullOrWhiteSpace(item.EmailAddress) &&
                    item.EmailAddress.Equals(emailAddress, StringComparison.InvariantCultureIgnoreCase));
    }
}
