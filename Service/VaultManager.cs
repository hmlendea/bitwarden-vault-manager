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

        public void PrintItemDetails()
        {
            BitwardenItem item = vault.Items.First(x =>
                x.FolderId != Guid.Empty &&
                x.Type == BitwardenItemType.Login);

            BitwardenFolder folder = vault.Folders.First(x => x.Id == item.FolderId);

            Console.WriteLine($"Id: {item.Name}");
            Console.WriteLine($"Name: {item.Name}");
            Console.WriteLine($"Type: {item.Type}");
            Console.WriteLine($"Folder: {folder.Name}");
            Console.WriteLine($"Username: {item.Login.Username}");
            Console.WriteLine($"Password: {item.Login.Password}");
            Console.WriteLine($"TOTP: {item.Login.TOTP}");
        }

        public IDictionary<string, int> GetEmailAddressUsageCounts()
            => vault.Items
                .Where(item => !string.IsNullOrWhiteSpace(item.EmailAddress))
                .Select(item => item.EmailAddress.ToLowerInvariant())
                .Distinct()
                .ToDictionary(
                    emailAddress => emailAddress,
                    emailAddress => vault.Items
                        .Where(item =>
                            !string.IsNullOrWhiteSpace(item.EmailAddress) &&
                            item.EmailAddress.Equals(emailAddress, StringComparison.InvariantCultureIgnoreCase))
                        .Count());
    }
}
